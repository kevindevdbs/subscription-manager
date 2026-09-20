using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Time.Testing;
using SubscriptionManager.Infrastructure.Data;
using Testcontainers.MsSql;

namespace WebApi.Tests;

public class SubscriptionManagerApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // Relógio fixo da suíte. A varredura de vencidas alcança toda fatura do banco
    // com vencimento antes desta data, e os testes precisam saber exatamente quais.
    public static readonly DateTimeOffset Now = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private readonly MsSqlContainer _sqlServerContainer;

    public SubscriptionManagerApplicationFactory()
    {
        _sqlServerContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Tests")
            .ConfigureAppConfiguration((_, configuration) =>
            {
                var parameters = new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = _sqlServerContainer.GetConnectionString(),
                    ["BillingJob:Enabled"] = "false"
                };

                configuration.AddInMemoryCollection(parameters);
            })
            .ConfigureTestServices(services =>
            {
                services.RemoveAll<TimeProvider>();
                services.AddSingleton<TimeProvider>(new FakeTimeProvider(Now));
            });
    }

    public async Task InitializeAsync()
    {
        await _sqlServerContainer.StartAsync();

        await using var scope = Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await dbContext.Database.MigrateAsync();
    }

    async Task IAsyncLifetime.DisposeAsync() => await _sqlServerContainer.DisposeAsync();
}

[CollectionDefinition(nameof(IntegrationTestCollection))]
public class IntegrationTestCollection : ICollectionFixture<SubscriptionManagerApplicationFactory>
{
}
