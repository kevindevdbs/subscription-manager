using CommonTestUtilities.Requests;
using Shouldly;
using SubscriptionManager.Application.DTOs.Contracts;
using System.Net;

namespace WebApi.Tests.Listing;

public class ListEndpointsTests : BaseIntegrationTest
{
    public ListEndpointsTests(SubscriptionManagerApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ListCustomers_ShouldIncludeTheCustomerJustCreated()
    {
        var request = CreateCustomerRequestBuilder.Build();

        var created = await Post("/api/customers", request);
        created.StatusCode.ShouldBe(HttpStatusCode.Created);

        var response = await Get("/api/customers");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var json = await ReadJson(response);

        var documents = json.RootElement
            .EnumerateArray()
            .Select(customer => customer.GetProperty("document").GetString())
            .ToList();

        documents.ShouldContain(request.Document);
    }

    [Fact]
    public async Task ListPlans_ShouldIncludeThePlanJustCreated()
    {
        var request = CreatePlanRequestBuilder.Build();

        var created = await Post("/api/plans", request);
        created.StatusCode.ShouldBe(HttpStatusCode.Created);

        var response = await Get("/api/plans");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var json = await ReadJson(response);

        var names = json.RootElement
            .EnumerateArray()
            .Select(plan => plan.GetProperty("name").GetString())
            .ToList();

        names.ShouldContain(request.Name);
    }

    [Fact]
    public async Task ListContracts_ShouldIncludeTheContractJustCreated()
    {
        var customerResponse = await Post("/api/customers", CreateCustomerRequestBuilder.Build());
        using var customerJson = await ReadJson(customerResponse);
        var customerId = customerJson.RootElement.GetProperty("id").GetGuid();

        var planResponse = await Post("/api/plans", CreatePlanRequestBuilder.Build());
        using var planJson = await ReadJson(planResponse);
        var planId = planJson.RootElement.GetProperty("id").GetGuid();

        var contractResponse = await Post("/api/contracts", new CreateContractRequest(customerId, planId, new DateTime(2026, 9, 1)));
        using var contractJson = await ReadJson(contractResponse);
        var contractId = contractJson.RootElement.GetProperty("id").GetGuid();

        var response = await Get("/api/contracts");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var json = await ReadJson(response);

        var ids = json.RootElement
            .EnumerateArray()
            .Select(contract => contract.GetProperty("id").GetGuid())
            .ToList();

        ids.ShouldContain(contractId);
    }
}
