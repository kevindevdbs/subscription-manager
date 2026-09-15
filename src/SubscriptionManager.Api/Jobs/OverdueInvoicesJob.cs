using Microsoft.Extensions.Options;
using SubscriptionManager.Api.Configuration;
using SubscriptionManager.Application.UseCases.Invoices;

namespace SubscriptionManager.Api.Jobs;

public class OverdueInvoicesJob : BackgroundService
{
    private readonly ILogger<OverdueInvoicesJob> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IOptions<OverdueInvoicesJobOptions> _options;

    public OverdueInvoicesJob(ILogger<OverdueInvoicesJob> logger, IServiceScopeFactory serviceScopeFactory, IOptions<OverdueInvoicesJobOptions> options)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _options = options;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Value.Enabled)
        {
            return;
        }

        await RunOnce(stoppingToken);

        using var timer = new PeriodicTimer(_options.Value.Interval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await RunOnce(stoppingToken);
        }
    }

    private async Task RunOnce(CancellationToken stoppingToken)
    {
        try
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();

            var handler = scope.ServiceProvider.GetRequiredService<MarkOverdueInvoicesHandler>();

            var count = await handler.Handle();

            _logger.LogInformation("{Count} fatura(s) marcada(s) como vencida(s).", count);
        }
        catch (Exception exception) when (!stoppingToken.IsCancellationRequested)
        {
            // Sem este catch, uma falha do banco derrubaria a API inteira.
            _logger.LogError(exception, "Falha ao marcar faturas vencidas. Nova tentativa no próximo ciclo.");
        }
    }
}
