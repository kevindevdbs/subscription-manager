using CommonTestUtilities.Requests;
using Shouldly;
using SubscriptionManager.Application.DTOs.Contracts;
using System.Net;

namespace WebApi.Tests.Transitions;

public class ContractTransitionTests : BaseIntegrationTest
{
    // Data no passado para que o encerramento sem corpo (que vale "agora")
    // caia depois do início do contrato.
    private static readonly DateTime StartDate = new(2026, 1, 1);

    public ContractTransitionTests(SubscriptionManagerApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Suspend_Success()
    {
        var contractId = await CreateContract();

        var response = await Patch($"/api/contracts/{contractId}/suspend");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var json = await ReadJson(response);

        json.RootElement.GetProperty("status").GetString().ShouldBe("Suspended");
    }

    [Fact]
    public async Task Suspend_ShouldReturnConflict_WhenContractIsAlreadySuspended()
    {
        var contractId = await CreateContract();

        await Patch($"/api/contracts/{contractId}/suspend");

        var response = await Patch($"/api/contracts/{contractId}/suspend");

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);

        var errors = await ReadErrors(response);

        errors.ShouldContain("O contrato não está ativo e não pode ser suspenso.");
    }

    [Fact]
    public async Task Suspend_ShouldReturnNotFound_WhenContractDoesNotExist()
    {
        var response = await Patch($"/api/contracts/{Guid.NewGuid()}/suspend");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var errors = await ReadErrors(response);

        errors.ShouldContain("Contrato não encontrado.");
    }

    [Fact]
    public async Task Reactivate_Success()
    {
        var contractId = await CreateContract();

        await Patch($"/api/contracts/{contractId}/suspend");

        var response = await Patch($"/api/contracts/{contractId}/reactivate");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var json = await ReadJson(response);

        json.RootElement.GetProperty("status").GetString().ShouldBe("Active");
    }

    [Fact]
    public async Task Reactivate_ShouldReturnConflict_WhenContractIsActive()
    {
        var contractId = await CreateContract();

        var response = await Patch($"/api/contracts/{contractId}/reactivate");

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);

        var errors = await ReadErrors(response);

        errors.ShouldContain("O contrato não está suspenso e não pode ser ativado.");
    }

    [Fact]
    public async Task Cancel_Success()
    {
        var contractId = await CreateContract();

        var endDate = StartDate.AddMonths(6);

        var response = await Patch($"/api/contracts/{contractId}/cancel", new CancelContractRequest(endDate));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var json = await ReadJson(response);

        json.RootElement.GetProperty("status").GetString().ShouldBe("Cancelled");
        json.RootElement.GetProperty("endDate").GetDateTime().ShouldBe(endDate);
    }

    [Fact]
    public async Task Cancel_ShouldReturnConflict_WhenContractIsAlreadyCancelled()
    {
        var contractId = await CreateContract();

        await Patch($"/api/contracts/{contractId}/cancel", new CancelContractRequest(StartDate.AddMonths(1)));

        var response = await Patch($"/api/contracts/{contractId}/cancel", new CancelContractRequest(StartDate.AddMonths(2)));

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);

        var errors = await ReadErrors(response);

        errors.ShouldContain("O contrato já está cancelado.");
    }

    [Fact]
    public async Task Cancel_ShouldReturnBadRequest_WhenEndDateIsBeforeStartDate()
    {
        var contractId = await CreateContract();

        var response = await Patch($"/api/contracts/{contractId}/cancel", new CancelContractRequest(StartDate.AddDays(-1)));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errors = await ReadErrors(response);

        errors.ShouldContain("A data de encerramento não pode ser anterior à data de início.");
    }

    [Fact]
    public async Task Cancel_ShouldUseTheCurrentDate_WhenTheBodyIsOmitted()
    {
        var contractId = await CreateContract();

        var before = DateTime.UtcNow;

        var response = await Patch($"/api/contracts/{contractId}/cancel");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var json = await ReadJson(response);

        json.RootElement.GetProperty("status").GetString().ShouldBe("Cancelled");
        json.RootElement.GetProperty("endDate").GetDateTime().ShouldBeInRange(before, DateTime.UtcNow);
    }

    [Fact]
    public async Task Cancel_ShouldReturnBadRequest_WhenEndDateIsInformedButEmpty()
    {
        var contractId = await CreateContract();

        var response = await Patch($"/api/contracts/{contractId}/cancel", new CancelContractRequest(default(DateTime)));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errors = await ReadErrors(response);

        errors.ShouldContain("A data de encerramento é inválida.");
    }

    private async Task<Guid> CreateContract()
    {
        var customerResponse = await Post("/api/customers", CreateCustomerRequestBuilder.Build());
        using var customerJson = await ReadJson(customerResponse);
        var customerId = customerJson.RootElement.GetProperty("id").GetGuid();

        var planResponse = await Post("/api/plans", CreatePlanRequestBuilder.Build());
        using var planJson = await ReadJson(planResponse);
        var planId = planJson.RootElement.GetProperty("id").GetGuid();

        var contractResponse = await Post("/api/contracts", new CreateContractRequest(customerId, planId, StartDate));
        using var contractJson = await ReadJson(contractResponse);

        return contractJson.RootElement.GetProperty("id").GetGuid();
    }
}
