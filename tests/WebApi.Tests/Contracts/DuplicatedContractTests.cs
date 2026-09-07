using CommonTestUtilities.Requests;
using Shouldly;
using SubscriptionManager.Application.DTOs.Contracts;
using System.Net;

namespace WebApi.Tests.Contracts;

public class DuplicatedContractTests : BaseIntegrationTest
{
    private static readonly DateTime StartDate = new(2029, 1, 1);

    public DuplicatedContractTests(SubscriptionManagerApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Create_ShouldReturnConflict_WhenTheSameCustomerSignsTheSamePlanTwice()
    {
        var (customerId, planId) = await CreateCustomerAndPlan();

        var first = await Post("/api/contracts", new CreateContractRequest(customerId, planId, StartDate));
        first.StatusCode.ShouldBe(HttpStatusCode.Created);

        var second = await Post("/api/contracts", new CreateContractRequest(customerId, planId, StartDate));

        second.StatusCode.ShouldBe(HttpStatusCode.Conflict);

        var errors = await ReadErrors(second);

        errors.ShouldContain("Este cliente já possui um contrato aberto para este plano.");
    }

    [Fact]
    public async Task Create_ShouldAllowTheSameCustomerOnADifferentPlan()
    {
        var (customerId, planId) = await CreateCustomerAndPlan();

        await Post("/api/contracts", new CreateContractRequest(customerId, planId, StartDate));

        var otherPlanResponse = await Post("/api/plans", CreatePlanRequestBuilder.Build());
        using var otherPlanJson = await ReadJson(otherPlanResponse);
        var otherPlanId = otherPlanJson.RootElement.GetProperty("id").GetGuid();

        var response = await Post("/api/contracts", new CreateContractRequest(customerId, otherPlanId, StartDate));

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_ShouldAllowTheSamePlanForADifferentCustomer()
    {
        var (customerId, planId) = await CreateCustomerAndPlan();

        await Post("/api/contracts", new CreateContractRequest(customerId, planId, StartDate));

        var otherCustomerResponse = await Post("/api/customers", CreateCustomerRequestBuilder.Build());
        using var otherCustomerJson = await ReadJson(otherCustomerResponse);
        var otherCustomerId = otherCustomerJson.RootElement.GetProperty("id").GetGuid();

        var response = await Post("/api/contracts", new CreateContractRequest(otherCustomerId, planId, StartDate));

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_ShouldAllowResigningThePlan_AfterTheFirstContractWasCancelled()
    {
        var (customerId, planId) = await CreateCustomerAndPlan();

        var first = await Post("/api/contracts", new CreateContractRequest(customerId, planId, StartDate));
        using var firstJson = await ReadJson(first);
        var contractId = firstJson.RootElement.GetProperty("id").GetGuid();

        await Patch($"/api/contracts/{contractId}/cancel", new CancelContractRequest(StartDate.AddMonths(3)));

        var response = await Post("/api/contracts", new CreateContractRequest(customerId, planId, StartDate.AddMonths(4)));

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_ShouldReturnConflict_WhenTheOpenContractIsSuspended()
    {
        var (customerId, planId) = await CreateCustomerAndPlan();

        var first = await Post("/api/contracts", new CreateContractRequest(customerId, planId, StartDate));
        using var firstJson = await ReadJson(first);
        var contractId = firstJson.RootElement.GetProperty("id").GetGuid();

        await Patch($"/api/contracts/{contractId}/suspend");

        var response = await Post("/api/contracts", new CreateContractRequest(customerId, planId, StartDate));

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    private async Task<(Guid CustomerId, Guid PlanId)> CreateCustomerAndPlan()
    {
        var customerResponse = await Post("/api/customers", CreateCustomerRequestBuilder.Build());
        using var customerJson = await ReadJson(customerResponse);

        var planResponse = await Post("/api/plans", CreatePlanRequestBuilder.Build());
        using var planJson = await ReadJson(planResponse);

        return (
            customerJson.RootElement.GetProperty("id").GetGuid(),
            planJson.RootElement.GetProperty("id").GetGuid());
    }
}
