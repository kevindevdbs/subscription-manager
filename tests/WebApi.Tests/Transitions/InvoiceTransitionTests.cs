using CommonTestUtilities.Requests;
using Shouldly;
using SubscriptionManager.Application.DTOs.Contracts;
using SubscriptionManager.Application.DTOs.Invoices;
using System.Net;

namespace WebApi.Tests.Transitions;

public class InvoiceTransitionTests : BaseIntegrationTest
{
    public InvoiceTransitionTests(SubscriptionManagerApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Pay_Success()
    {
        var invoice = await CreateInvoice(new DateTime(2028, 1, 1));

        var response = await Patch($"/api/invoices/{invoice.Id}/pay", new PayInvoiceRequest(invoice.DueDate));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var json = await ReadJson(response);

        json.RootElement.GetProperty("status").GetString().ShouldBe("Paid");
        json.RootElement.GetProperty("paidAt").GetDateTime().ShouldBe(invoice.DueDate);
    }

    [Fact]
    public async Task Pay_ShouldReturnConflict_WhenInvoiceIsAlreadyPaid()
    {
        var invoice = await CreateInvoice(new DateTime(2028, 2, 1));

        await Patch($"/api/invoices/{invoice.Id}/pay", new PayInvoiceRequest(invoice.DueDate));

        var response = await Patch($"/api/invoices/{invoice.Id}/pay", new PayInvoiceRequest(invoice.DueDate));

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);

        var errors = await ReadErrors(response);

        errors.ShouldContain("A fatura não está em um estado válido para ser paga.");
    }

    [Fact]
    public async Task Pay_ShouldReturnNotFound_WhenInvoiceDoesNotExist()
    {
        var response = await Patch($"/api/invoices/{Guid.NewGuid()}/pay", new PayInvoiceRequest(new DateTime(2028, 1, 10)));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var errors = await ReadErrors(response);

        errors.ShouldContain("Fatura não encontrada.");
    }

    [Fact]
    public async Task Pay_ShouldUseTheCurrentDate_WhenTheBodyIsOmitted()
    {
        var invoice = await CreateInvoice(new DateTime(2028, 3, 1));

        var before = DateTime.UtcNow;

        var response = await Patch($"/api/invoices/{invoice.Id}/pay");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var json = await ReadJson(response);

        json.RootElement.GetProperty("status").GetString().ShouldBe("Paid");
        json.RootElement.GetProperty("paidAt").GetDateTime().ShouldBeInRange(before, DateTime.UtcNow);
    }

    [Fact]
    public async Task Pay_ShouldReturnBadRequest_WhenPaidAtIsInformedButEmpty()
    {
        var invoice = await CreateInvoice(new DateTime(2028, 10, 1));

        var response = await Patch($"/api/invoices/{invoice.Id}/pay", new PayInvoiceRequest(default(DateTime)));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errors = await ReadErrors(response);

        errors.ShouldContain("A data do pagamento é inválida.");
    }

    [Fact]
    public async Task Overdue_Success()
    {
        var invoice = await CreateInvoice(new DateTime(2028, 4, 1));

        var request = new MarkInvoiceAsOverdueRequest(invoice.DueDate.AddDays(1));

        var response = await Patch($"/api/invoices/{invoice.Id}/overdue", request);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var json = await ReadJson(response);

        json.RootElement.GetProperty("status").GetString().ShouldBe("Overdue");
    }

    [Fact]
    public async Task Overdue_ShouldKeepItPending_WhenTheDueDateHasNotPassedYet()
    {
        var invoice = await CreateInvoice(new DateTime(2028, 5, 1));

        var request = new MarkInvoiceAsOverdueRequest(invoice.DueDate.AddDays(-1));

        var response = await Patch($"/api/invoices/{invoice.Id}/overdue", request);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var json = await ReadJson(response);

        json.RootElement.GetProperty("status").GetString().ShouldBe("Pending");
    }

    [Fact]
    public async Task Refund_Success()
    {
        var invoice = await CreateInvoice(new DateTime(2028, 6, 1));

        await Patch($"/api/invoices/{invoice.Id}/pay", new PayInvoiceRequest(invoice.DueDate));

        var response = await Patch($"/api/invoices/{invoice.Id}/refund");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var json = await ReadJson(response);

        json.RootElement.GetProperty("status").GetString().ShouldBe("Refunded");
    }

    [Fact]
    public async Task Refund_ShouldReturnConflict_WhenInvoiceWasNeverPaid()
    {
        var invoice = await CreateInvoice(new DateTime(2028, 7, 1));

        var response = await Patch($"/api/invoices/{invoice.Id}/refund");

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);

        var errors = await ReadErrors(response);

        errors.ShouldContain("A fatura não está em um estado válido para ser reembolsada.");
    }

    [Fact]
    public async Task Cancel_Success()
    {
        var invoice = await CreateInvoice(new DateTime(2028, 8, 1));

        var response = await Patch($"/api/invoices/{invoice.Id}/cancel");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var json = await ReadJson(response);

        json.RootElement.GetProperty("status").GetString().ShouldBe("Cancelled");
    }

    [Fact]
    public async Task Cancel_ShouldReturnConflict_WhenInvoiceIsAlreadyPaid()
    {
        var invoice = await CreateInvoice(new DateTime(2028, 9, 1));

        await Patch($"/api/invoices/{invoice.Id}/pay", new PayInvoiceRequest(invoice.DueDate));

        var response = await Patch($"/api/invoices/{invoice.Id}/cancel");

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);

        var errors = await ReadErrors(response);

        errors.ShouldContain("A fatura não está em um estado válido para ser cancelada.");
    }

    // A varredura é global: pega toda fatura pendente vencida do banco, que é
    // compartilhado pela suíte. Por isso os testes de lote usam meses anteriores
    // a 2027 — o resto da suíte só trabalha de 2027 em diante, então nenhuma
    // varredura daqui alcança a fatura de outro teste, em qualquer ordem.

    [Fact]
    public async Task MarkOverdue_ShouldSweepEveryPendingInvoiceAlreadyDue()
    {
        var invoice = await CreateInvoice(new DateTime(2018, 1, 1));

        var response = await Post("/api/invoices/mark-overdue", new MarkInvoiceAsOverdueRequest(invoice.DueDate.AddDays(1)));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var json = await ReadJson(response);

        json.RootElement.GetProperty("markedAsOverdue").GetInt32().ShouldBeGreaterThan(0);

        var after = await Get("/api/invoices?status=Overdue&month=2018-01");
        using var afterJson = await ReadJson(after);

        afterJson.RootElement
            .EnumerateArray()
            .ShouldContain(item => item.GetProperty("id").GetGuid() == invoice.Id);
    }

    [Fact]
    public async Task MarkOverdue_ShouldLeaveInvoicesThatAreNotDueYet()
    {
        var invoice = await CreateInvoice(new DateTime(2019, 1, 1));

        var response = await Post("/api/invoices/mark-overdue", new MarkInvoiceAsOverdueRequest(invoice.DueDate.AddDays(-1)));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var stillPending = await Get($"/api/contracts/{invoice.ContractId}/invoices");
        using var json = await ReadJson(stillPending);

        json.RootElement
            .EnumerateArray()
            .First()
            .GetProperty("status")
            .GetString()
            .ShouldBe("Pending");
    }

    [Fact]
    public async Task MarkOverdue_ShouldNotTouchAnInvoiceThatIsAlreadyPaid()
    {
        var invoice = await CreateInvoice(new DateTime(2020, 1, 1));

        await Patch($"/api/invoices/{invoice.Id}/pay", new PayInvoiceRequest(invoice.DueDate));

        var response = await Post("/api/invoices/mark-overdue", new MarkInvoiceAsOverdueRequest(invoice.DueDate.AddDays(1)));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var after = await Get($"/api/contracts/{invoice.ContractId}/invoices");
        using var json = await ReadJson(after);

        json.RootElement
            .EnumerateArray()
            .First()
            .GetProperty("status")
            .GetString()
            .ShouldBe("Paid");
    }

    [Fact]
    public async Task MarkOverdue_ShouldReturnBadRequest_WhenReferenceDateIsInformedButEmpty()
    {
        var response = await Post("/api/invoices/mark-overdue", new MarkInvoiceAsOverdueRequest(default(DateTime)));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errors = await ReadErrors(response);

        errors.ShouldContain("A data de referência é inválida.");
    }

    /// <summary>
    /// Cria cliente, plano e contrato novos e gera a fatura do mês pedido,
    /// devolvendo a fatura desse contrato.
    /// </summary>
    private async Task<InvoiceResponse> CreateInvoice(DateTime referenceMonth)
    {
        var customerResponse = await Post("/api/customers", CreateCustomerRequestBuilder.Build());
        using var customerJson = await ReadJson(customerResponse);
        var customerId = customerJson.RootElement.GetProperty("id").GetGuid();

        var planResponse = await Post("/api/plans", CreatePlanRequestBuilder.Build());
        using var planJson = await ReadJson(planResponse);
        var planId = planJson.RootElement.GetProperty("id").GetGuid();

        var contractResponse = await Post("/api/contracts", new CreateContractRequest(customerId, planId, referenceMonth));
        using var contractJson = await ReadJson(contractResponse);
        var contractId = contractJson.RootElement.GetProperty("id").GetGuid();

        await Post("/api/invoices/generate", new GenerateInvoiceRequest(referenceMonth));

        var invoicesResponse = await Get($"/api/contracts/{contractId}/invoices");
        using var invoicesJson = await ReadJson(invoicesResponse);

        var invoice = invoicesJson.RootElement.EnumerateArray().First();

        return new InvoiceResponse(
            invoice.GetProperty("id").GetGuid(),
            invoice.GetProperty("contractId").GetGuid(),
            invoice.GetProperty("amount").GetDecimal(),
            invoice.GetProperty("dueDate").GetDateTime(),
            invoice.GetProperty("referenceMonth").GetDateTime(),
            null,
            invoice.GetProperty("status").GetString()!);
    }
}
