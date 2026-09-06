using CommonTestUtilities.Requests;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using SubscriptionManager.Application.DTOs.Contracts;
using System.Net;

namespace WebApi.Tests.Contracts;

public class ContractTests : BaseIntegrationTest
{
    private const string RequestUri = "/api/contracts";

    public ContractTests(SubscriptionManagerApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Success()
    {
        var customerId = await CreateCustomer();
        var planId = await CreatePlan();

        var request = new CreateContractRequest(customerId, planId, new DateTime(2026, 9, 1));

        var response = await Post(RequestUri, request);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        using var json = await ReadJson(response);

        json.RootElement.GetProperty("customerId").GetGuid().ShouldBe(customerId);
        json.RootElement.GetProperty("planId").GetGuid().ShouldBe(planId);
        json.RootElement.GetProperty("status").GetString().ShouldBe("Active");

        var contractId = json.RootElement.GetProperty("id").GetGuid();

        var persisted = await DbContext.Contracts.AnyAsync(contract => contract.Id == contractId);

        persisted.ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenCustomerDoesNotExist()
    {
        var planId = await CreatePlan();

        var request = new CreateContractRequest(Guid.NewGuid(), planId, new DateTime(2026, 9, 1));

        var response = await Post(RequestUri, request);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var errors = await ReadErrors(response);

        errors.ShouldContain("Cliente não encontrado");
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenPlanDoesNotExist()
    {
        var customerId = await CreateCustomer();

        var request = new CreateContractRequest(customerId, Guid.NewGuid(), new DateTime(2026, 9, 1));

        var response = await Post(RequestUri, request);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var errors = await ReadErrors(response);

        errors.ShouldContain("Plano não encontrado");
    }

    [Fact]
    public async Task GetById_Success()
    {
        var customerId = await CreateCustomer();
        var planId = await CreatePlan();

        var created = await Post(RequestUri, new CreateContractRequest(customerId, planId, new DateTime(2026, 9, 1)));

        using var createdJson = await ReadJson(created);

        var contractId = createdJson.RootElement.GetProperty("id").GetGuid();

        var response = await Get($"{RequestUri}/{contractId}");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var json = await ReadJson(response);

        json.RootElement.GetProperty("id").GetGuid().ShouldBe(contractId);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenContractDoesNotExist()
    {
        var response = await Get($"{RequestUri}/{Guid.NewGuid()}");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var errors = await ReadErrors(response);

        errors.ShouldContain("Contrato não encontrado.");
    }

    private async Task<Guid> CreateCustomer()
    {
        var response = await Post("/api/customers", CreateCustomerRequestBuilder.Build());

        using var json = await ReadJson(response);

        return json.RootElement.GetProperty("id").GetGuid();
    }

    private async Task<Guid> CreatePlan()
    {
        var response = await Post("/api/plans", CreatePlanRequestBuilder.Build());

        using var json = await ReadJson(response);

        return json.RootElement.GetProperty("id").GetGuid();
    }
}
