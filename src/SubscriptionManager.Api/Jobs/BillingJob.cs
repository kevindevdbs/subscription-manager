using Microsoft.Extensions.Options;
using SubscriptionManager.Api.Configuration;
using SubscriptionManager.Application.DTOs.Invoices;
using SubscriptionManager.Application.UseCases.Invoices;

namespace SubscriptionManager.Api.Jobs;

/// <summary>
/// Rotina diária de faturamento: emite as faturas do mês corrente e, na sequência,
/// marca as vencidas. A ordem importa — varrer depois de gerar garante que uma
/// fatura recém-emitida com vencimento já passado vença no mesmo ciclo, em vez de
/// esperar o próximo. Roda uma vez na subida e depois a cada intervalo.
/// </summary>
public class BillingJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly BillingJobOptions _options;
    private readonly ILogger<BillingJob> _logger;

    public BillingJob(
        IServiceScopeFactory scopeFactory,
        IOptions<BillingJobOptions> options,
        ILogger<BillingJob> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            return;
        }

        await RunOnce(stoppingToken);

        using var timer = new PeriodicTimer(_options.Interval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await RunOnce(stoppingToken);
        }
    }

    private async Task RunOnce(CancellationToken stoppingToken)
    {
        try
        {
            // O job vive enquanto a API estiver no ar, mas handler, repositório e
            // DbContext são scoped: cada execução abre o próprio escopo.
            await using var scope = _scopeFactory.CreateAsyncScope();
            var services = scope.ServiceProvider;

            var today = services.GetRequiredService<TimeProvider>().GetUtcNow().UtcDateTime;
            var referenceMonth = new DateTime(today.Year, today.Month, 1);

            var generate = services.GetRequiredService<GenerateMonthlyInvoicesHandler>();
            var generated = await generate.Handle(new GenerateInvoiceRequest(referenceMonth));

            var markOverdue = services.GetRequiredService<MarkOverdueInvoicesHandler>();
            var overdue = await markOverdue.Handle();

            _logger.LogInformation(
                "Faturamento: {Generated} fatura(s) gerada(s) para {ReferenceMonth:yyyy-MM}, {Overdue} marcada(s) como vencida(s).",
                generated,
                referenceMonth,
                overdue);
        }
        catch (Exception exception) when (!stoppingToken.IsCancellationRequested)
        {
            // Sem este catch, uma falha do banco derrubaria a API inteira.
            _logger.LogError(exception, "Falha na rotina de faturamento. Nova tentativa no próximo ciclo.");
        }
    }
}
