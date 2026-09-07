using Microsoft.Extensions.DependencyInjection;
using SubscriptionManager.Infrastructure.Data;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebApi.Tests;

[Collection(nameof(IntegrationTestCollection))]
public abstract class BaseIntegrationTest : IDisposable
{
    internal readonly AppDbContext DbContext;

    private readonly HttpClient _httpClient;
    private readonly IServiceScope _scope;

    protected BaseIntegrationTest(SubscriptionManagerApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();

        _scope = factory.Services.CreateScope();

        DbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    protected async Task<HttpResponseMessage> Post(string requestUri, object request)
    {
        return await _httpClient.PostAsJsonAsync(requestUri, request);
    }

    protected async Task<HttpResponseMessage> Get(string requestUri)
    {
        return await _httpClient.GetAsync(requestUri);
    }

    protected async Task<HttpResponseMessage> Patch(string requestUri, object? request = null)
    {
        return await _httpClient.PatchAsJsonAsync(requestUri, request);
    }

    protected static async Task<JsonDocument> ReadJson(HttpResponseMessage response)
    {
        await using var body = await response.Content.ReadAsStreamAsync();

        return await JsonDocument.ParseAsync(body);
    }

    protected static async Task<List<string>> ReadErrors(HttpResponseMessage response)
    {
        using var json = await ReadJson(response);

        return json.RootElement
            .GetProperty("errors")
            .EnumerateArray()
            .Select(error => error.GetString() ?? string.Empty)
            .ToList();
    }

    public void Dispose()
    {
        _scope?.Dispose();
        GC.SuppressFinalize(this);
    }
}
