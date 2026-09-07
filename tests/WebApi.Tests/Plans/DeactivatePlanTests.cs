using CommonTestUtilities.Requests;
using Shouldly;
using SubscriptionManager.Application.DTOs.Contracts;
using System.Net;

namespace WebApi.Tests.Plans;

public class DeactivatePlanTests : BaseIntegrationTest
{
    public DeactivatePlanTests(SubscriptionManagerApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Deactivate_Success()
    {
        var planId = await CreatePlan();

        var response = await Patch($"/api/plans/{planId}/deactivate");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var json = await ReadJson(response);

        json.RootElement.GetProperty("isActive").GetBoolean().ShouldBeFalse();
    }

    [Fact]
    public async Task Deactivate_ShouldReturnConflict_WhenPlanIsAlreadyDeactivated()
    {
        var planId = await CreatePlan();

        await Patch($"/api/plans/{planId}/deactivate");

        var response = await Patch($"/api/plans/{planId}/deactivate");

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);

        var errors = await ReadErrors(response);

        errors.ShouldContain("O plano já está desativado.");
    }

    [Fact]
    public async Task Deactivate_ShouldReturnNotFound_WhenPlanDoesNotExist()
    {
        var response = await Patch($"/api/plans/{Guid.NewGuid()}/deactivate");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var errors = await ReadErrors(response);

        errors.ShouldContain("Plano não encontrado.");
    }

    [Fact]
    public async Task CreateContract_ShouldReturnConflict_WhenThePlanIsDeactivated()
    {
        var planId = await CreatePlan();
        var customerId = await CreateCustomer();

        await Patch($"/api/plans/{planId}/deactivate");

        var response = await Post("/api/contracts", new CreateContractRequest(customerId, planId, new DateTime(2026, 1, 1)));

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);

        var errors = await ReadErrors(response);

        errors.ShouldContain("Este plano está desativado e não aceita novos contratos.");
    }

    /// <summary>
    /// Contrato assinado antes da desativação continua valendo e continua sendo
    /// faturado — desativar o plano só fecha a porta para adesão nova.
    /// </summary>
    [Fact]
    public async Task Deactivate_ShouldKeepBillingTheContractsSignedBefore()
    {
        var referenceMonth = new DateTime(2031, 1, 1);

        var planId = await CreatePlan();
        var customerId = await CreateCustomer();

        var contractResponse = await Post("/api/contracts", new CreateContractRequest(customerId, planId, referenceMonth));
        using var contractJson = await ReadJson(contractResponse);
        var contractId = contractJson.RootElement.GetProperty("id").GetGuid();

        await Patch($"/api/plans/{planId}/deactivate");

        await Post("/api/invoices/generate", new { referenceMonth });

        var invoices = await Get($"/api/contracts/{contractId}/invoices");
        using var invoicesJson = await ReadJson(invoices);

        invoicesJson.RootElement.EnumerateArray().ShouldNotBeEmpty();
    }

    private async Task<Guid> CreatePlan()
    {
        var response = await Post("/api/plans", CreatePlanRequestBuilder.Build());
        using var json = await ReadJson(response);

        return json.RootElement.GetProperty("id").GetGuid();
    }

    private async Task<Guid> CreateCustomer()
    {
        var response = await Post("/api/customers", CreateCustomerRequestBuilder.Build());
        using var json = await ReadJson(response);

        return json.RootElement.GetProperty("id").GetGuid();
    }
}
