using Microsoft.Extensions.Options;
using SubscriptionManager.Api.Configuration;
using SubscriptionManager.Application.UseCases.Invoices;

namespace SubscriptionManager.Api.Jobs;

public class OverdueInvoicesJob : RecurringJob
{
    public OverdueInvoicesJob(IServiceScopeFactory serviceScopeFactory, IOptions<OverdueInvoicesJobOptions> options, ILogger<OverdueInvoicesJob> logger)
        : base(serviceScopeFactory, options.Value, logger)
    {
    }

    protected override async Task RunAsync(IServiceProvider services)
    {
        var handler = services.GetRequiredService<MarkOverdueInvoicesHandler>();

        var count = await handler.Handle();

        Logger.LogInformation("{Count} fatura(s) marcada(s) como vencida(s).", count);
    }
}
