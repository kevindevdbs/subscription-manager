using CommonTestUtilities.Requests;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using SubscriptionManager.Application.DTOs.Contracts;
using SubscriptionManager.Application.DTOs.Invoices;
using System.Net;

namespace WebApi.Tests.Invoices;

public class InvoiceTests : BaseIntegrationTest
{
    public InvoiceTests(SubscriptionManagerApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Generate_ShouldCreateOneInvoiceForTheContract()
    {
        var referenceMonth = new DateTime(2027, 1, 1);

        var contractId = await CreateContract();

        var response = await Post("/api/invoices/generate", new GenerateInvoiceRequest(referenceMonth));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var invoices = await DbContext.Invoices
            .Where(invoice => invoice.ContractId == contractId && invoice.ReferenceMonth == referenceMonth)
            .CountAsync();

        invoices.ShouldBe(1);
    }

    [Fact]
    public async Task Generate_ShouldBeIdempotent_WhenRunTwiceForTheSameMonth()
    {
        var referenceMonth = new DateTime(2027, 2, 1);

        await CreateContract();

        var firstRun = await Post("/api/invoices/generate", new GenerateInvoiceRequest(referenceMonth));

        using var firstJson = await ReadJson(firstRun);
        firstJson.RootElement.GetProperty("generated").GetInt32().ShouldBeGreaterThan(0);

        var secondRun = await Post("/api/invoices/generate", new GenerateInvoiceRequest(referenceMonth));

        using var secondJson = await ReadJson(secondRun);
        secondJson.RootElement.GetProperty("generated").GetInt32().ShouldBe(0);
    }

    [Fact]
    public async Task Generate_ShouldReturnBadRequest_WhenReferenceMonthIsMissing()
    {
        var response = await Post("/api/invoices/generate", new GenerateInvoiceRequest(default));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errors = await ReadErrors(response);

        errors.ShouldContain("O mês de referência é obrigatório.");
    }

    [Fact]
    public async Task GetByContract_Success()
    {
        var referenceMonth = new DateTime(2027, 3, 1);

        var contractId = await CreateContract();

        await Post("/api/invoices/generate", new GenerateInvoiceRequest(referenceMonth));

        var response = await Get($"/api/contracts/{contractId}/invoices");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var json = await ReadJson(response);

        var invoices = json.RootElement.EnumerateArray().ToList();

        invoices.ShouldNotBeEmpty();
        invoices.ShouldAllBe(invoice => invoice.GetProperty("contractId").GetGuid() == contractId);
    }

    [Fact]
    public async Task GetByContract_ShouldReturnNotFound_WhenContractDoesNotExist()
    {
        var response = await Get($"/api/contracts/{Guid.NewGuid()}/invoices");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var errors = await ReadErrors(response);

        errors.ShouldContain("Contrato não encontrado.");
    }

    [Fact]
    public async Task List_ShouldFilterByStatusAndMonth()
    {
        var referenceMonth = new DateTime(2027, 4, 1);

        await CreateContract();

        await Post("/api/invoices/generate", new GenerateInvoiceRequest(referenceMonth));

        var response = await Get("/api/invoices?status=Pending&month=2027-04");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var json = await ReadJson(response);

        var invoices = json.RootElement.EnumerateArray().ToList();

        invoices.ShouldNotBeEmpty();
        invoices.ShouldAllBe(invoice => invoice.GetProperty("status").GetString() == "Pending");
        invoices.ShouldAllBe(invoice => invoice.GetProperty("referenceMonth").GetDateTime() == referenceMonth);
    }

    [Fact]
    public async Task List_ShouldReturnEmpty_WhenNoInvoiceMatchesTheFilter()
    {
        var response = await Get("/api/invoices?status=Refunded&month=2030-12");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var json = await ReadJson(response);

        json.RootElement.EnumerateArray().ShouldBeEmpty();
    }

    [Theory]
    [InlineData("/api/invoices?status=xpto", "Status inválido.")]
    [InlineData("/api/invoices?month=setembro", "Mês inválido.")]
    public async Task List_ShouldReturnBadRequest_WhenFilterIsInvalid(string requestUri, string expectedPrefix)
    {
        var response = await Get(requestUri);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errors = await ReadErrors(response);

        errors.ShouldContain(error => error.StartsWith(expectedPrefix));
    }

    private async Task<Guid> CreateContract()
    {
        var customerResponse = await Post("/api/customers", CreateCustomerRequestBuilder.Build());
        using var customerJson = await ReadJson(customerResponse);
        var customerId = customerJson.RootElement.GetProperty("id").GetGuid();

        var planResponse = await Post("/api/plans", CreatePlanRequestBuilder.Build());
        using var planJson = await ReadJson(planResponse);
        var planId = planJson.RootElement.GetProperty("id").GetGuid();

        var contractResponse = await Post("/api/contracts", new CreateContractRequest(customerId, planId, new DateTime(2026, 9, 1)));
        using var contractJson = await ReadJson(contractResponse);

        return contractJson.RootElement.GetProperty("id").GetGuid();
    }
}
