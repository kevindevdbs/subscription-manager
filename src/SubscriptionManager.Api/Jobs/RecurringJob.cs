using SubscriptionManager.Api.Configuration;

namespace SubscriptionManager.Api.Jobs;

/// <summary>
/// Base dos jobs recorrentes: roda uma vez na subida da API e depois a cada intervalo.
/// </summary>
public abstract class RecurringJob : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly RecurringJobOptions _options;

    protected RecurringJob(IServiceScopeFactory serviceScopeFactory, RecurringJobOptions options, ILogger logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _options = options;
        Logger = logger;
    }

    protected ILogger Logger { get; }

    protected abstract Task RunAsync(IServiceProvider services);

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
            await using var scope = _serviceScopeFactory.CreateAsyncScope();

            await RunAsync(scope.ServiceProvider);
        }
        catch (Exception exception) when (!stoppingToken.IsCancellationRequested)
        {
            // Sem este catch, uma falha do banco derrubaria a API inteira.
            Logger.LogError(exception, "Falha ao executar {Job}. Nova tentativa no próximo ciclo.", GetType().Name);
        }
    }
}
