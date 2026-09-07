using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using Shouldly;
using SubscriptionManager.Application.UseCases.Invoices;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Exceptions;
using System.Net;

namespace UseCases.Tests.Invoices;

public class RefundInvoiceHandlerTests
{
    [Fact]
    public async Task Success()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Pay(new DateTime(2026, 9, 15));

        var result = await CreateHandler(invoice).Handle(invoice.Id);

        result.Status.ShouldBe("Refunded");
    }

    [Fact]
    public async Task Handle_ShouldThrowConflict_WhenInvoiceWasNeverPaid()
    {
        var invoice = InvoiceBuilder.Build();

        var exception = await Should.ThrowAsync<ConflictException>(() => CreateHandler(invoice).Handle(invoice.Id));

        exception.GetStatusCode().ShouldBe(HttpStatusCode.Conflict);
        exception.GetErrorMessages().ShouldContain("A fatura não está em um estado válido para ser reembolsada.");
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenInvoiceDoesNotExist()
    {
        var exception = await Should.ThrowAsync<NotFoundException>(() => CreateHandler(null).Handle(Guid.NewGuid()));

        exception.GetErrorMessages().ShouldContain("Fatura não encontrada.");
    }

    private static RefundInvoiceHandler CreateHandler(Invoice? invoice)
    {
        var invoiceBuilder = new IInvoiceRepositoryBuilder();
        if (invoice is not null)
        {
            invoiceBuilder.GetById(invoice);
        }

        return new RefundInvoiceHandler(invoiceBuilder.Build(), IUnitOfWorkBuilder.Build());
    }
}
