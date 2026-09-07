using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using Shouldly;
using SubscriptionManager.Application.DTOs.Invoices;
using SubscriptionManager.Application.UseCases.Invoices;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Exceptions;
using System.Net;

namespace UseCases.Tests.Invoices;

public class PayInvoiceHandlerTests
{
    private static readonly DateTime PaidAt = new(2026, 9, 15);

    [Fact]
    public async Task Success()
    {
        var invoice = InvoiceBuilder.Build();

        var result = await CreateHandler(invoice).Handle(invoice.Id, new PayInvoiceRequest(PaidAt));

        result.Id.ShouldBe(invoice.Id);
        result.Status.ShouldBe("Paid");
        result.PaidAt.ShouldBe(PaidAt);
    }

    [Fact]
    public async Task Handle_ShouldPay_WhenInvoiceIsOverdue()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.MarkAsOverdue(invoice.DueDate.AddDays(1));

        var result = await CreateHandler(invoice).Handle(invoice.Id, new PayInvoiceRequest(PaidAt));

        result.Status.ShouldBe("Paid");
    }

    [Fact]
    public async Task Handle_ShouldThrowConflict_WhenInvoiceIsAlreadyPaid()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Pay(PaidAt);

        var exception = await Should.ThrowAsync<ConflictException>(
            () => CreateHandler(invoice).Handle(invoice.Id, new PayInvoiceRequest(PaidAt)));

        exception.GetStatusCode().ShouldBe(HttpStatusCode.Conflict);
        exception.GetErrorMessages().ShouldContain("A fatura não está em um estado válido para ser paga.");
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenInvoiceDoesNotExist()
    {
        var exception = await Should.ThrowAsync<NotFoundException>(
            () => CreateHandler(null).Handle(Guid.NewGuid(), new PayInvoiceRequest(PaidAt)));

        exception.GetErrorMessages().ShouldContain("Fatura não encontrada.");
    }

    [Fact]
    public async Task Handle_ShouldValidateBeforeTouchingTheRepository()
    {
        var exception = await Should.ThrowAsync<ErrorOnValidationException>(
            () => CreateHandler(null).Handle(Guid.NewGuid(), new PayInvoiceRequest(default)));

        exception.GetErrorMessages().ShouldContain("A data do pagamento é obrigatória.");
    }

    private static PayInvoiceHandler CreateHandler(Invoice? invoice)
    {
        var invoiceBuilder = new IInvoiceRepositoryBuilder();
        if (invoice is not null)
        {
            invoiceBuilder.GetById(invoice);
        }

        return new PayInvoiceHandler(invoiceBuilder.Build(), IUnitOfWorkBuilder.Build());
    }
}
