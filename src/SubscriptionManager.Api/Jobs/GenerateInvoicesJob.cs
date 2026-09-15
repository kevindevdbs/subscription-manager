using Microsoft.Extensions.Options;
using SubscriptionManager.Api.Configuration;
using SubscriptionManager.Application.DTOs.Invoices;
using SubscriptionManager.Application.UseCases.Invoices;

namespace SubscriptionManager.Api.Jobs;

public class GenerateInvoicesJob : RecurringJob
{
    public GenerateInvoicesJob(IServiceScopeFactory serviceScopeFactory, IOptions<GenerateInvoicesJobOptions> options, ILogger<GenerateInvoicesJob> logger)
        : base(serviceScopeFactory, options.Value, logger)
    {
    }

    protected override async Task RunAsync(IServiceProvider services)
    {
        var handler = services.GetRequiredService<GenerateMonthlyInvoicesHandler>();

        var today = services.GetRequiredService<TimeProvider>().GetUtcNow().UtcDateTime;
        var referenceMonth = new DateTime(today.Year, today.Month, 1);

        // Roda todo dia, e não só no dia 1, para alcançar também o contrato assinado
        // no meio do mês. A geração pula quem já tem fatura na competência.
        var count = await handler.Handle(new GenerateInvoiceRequest(referenceMonth));

        Logger.LogInformation("{Count} fatura(s) gerada(s) para {ReferenceMonth:yyyy-MM}.", count, referenceMonth);
    }
}
