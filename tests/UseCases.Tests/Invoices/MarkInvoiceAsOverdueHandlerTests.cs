using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using Shouldly;
using SubscriptionManager.Application.DTOs.Invoices;
using SubscriptionManager.Application.UseCases.Invoices;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Exceptions;
using System.Net;

namespace UseCases.Tests.Invoices;

public class MarkInvoiceAsOverdueHandlerTests
{
    [Fact]
    public async Task Success()
    {
        var invoice = InvoiceBuilder.Build();

        var request = new MarkInvoiceAsOverdueRequest(invoice.DueDate.AddDays(1));

        var result = await CreateHandler(invoice).Handle(invoice.Id, request);

        result.Status.ShouldBe("Overdue");
    }

    [Fact]
    public async Task Handle_ShouldKeepItPending_WhenTheDueDateHasNotPassedYet()
    {
        var invoice = InvoiceBuilder.Build();

        var request = new MarkInvoiceAsOverdueRequest(invoice.DueDate.AddDays(-1));

        var result = await CreateHandler(invoice).Handle(invoice.Id, request);

        result.Status.ShouldBe("Pending");
    }

    [Fact]
    public async Task Handle_ShouldThrowConflict_WhenInvoiceIsNotPending()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Pay(new DateTime(2026, 9, 15));

        var request = new MarkInvoiceAsOverdueRequest(invoice.DueDate.AddDays(1));

        var exception = await Should.ThrowAsync<ConflictException>(
            () => CreateHandler(invoice).Handle(invoice.Id, request));

        exception.GetStatusCode().ShouldBe(HttpStatusCode.Conflict);
        exception.GetErrorMessages().ShouldContain("A fatura não está em um estado válido para ser marcada como vencida.");
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenInvoiceDoesNotExist()
    {
        var request = new MarkInvoiceAsOverdueRequest(new DateTime(2026, 10, 1));

        var exception = await Should.ThrowAsync<NotFoundException>(
            () => CreateHandler(null).Handle(Guid.NewGuid(), request));

        exception.GetErrorMessages().ShouldContain("Fatura não encontrada.");
    }

    [Fact]
    public async Task Handle_ShouldFallBackToNow_WhenReferenceDateIsOmitted()
    {
        // Vencimento no passado, então "agora" já passou dele.
        var invoice = InvoiceBuilder.Build(referenceMonth: new DateTime(2020, 1, 1));

        var result = await CreateHandler(invoice).Handle(invoice.Id, new MarkInvoiceAsOverdueRequest());

        result.Status.ShouldBe("Overdue");
    }

    [Fact]
    public async Task Handle_ShouldFallBackToNow_WhenThereIsNoBodyAtAll()
    {
        var invoice = InvoiceBuilder.Build(referenceMonth: new DateTime(2020, 1, 1));

        var result = await CreateHandler(invoice).Handle(invoice.Id, null);

        result.Status.ShouldBe("Overdue");
    }

    [Fact]
    public async Task Handle_ShouldValidateBeforeTouchingTheRepository()
    {
        var exception = await Should.ThrowAsync<ErrorOnValidationException>(
            () => CreateHandler(null).Handle(Guid.NewGuid(), new MarkInvoiceAsOverdueRequest(default(DateTime))));

        exception.GetErrorMessages().ShouldContain("A data de referência é inválida.");
    }

    private static MarkInvoiceAsOverdueHandler CreateHandler(Invoice? invoice)
    {
        var invoiceBuilder = new IInvoiceRepositoryBuilder();
        if (invoice is not null)
        {
            invoiceBuilder.GetById(invoice);
        }

        return new MarkInvoiceAsOverdueHandler(invoiceBuilder.Build(), IUnitOfWorkBuilder.Build());
    }
}
