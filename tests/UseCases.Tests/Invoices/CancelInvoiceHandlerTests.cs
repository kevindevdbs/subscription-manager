using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using Shouldly;
using SubscriptionManager.Application.UseCases.Invoices;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Exceptions;
using System.Net;

namespace UseCases.Tests.Invoices;

public class CancelInvoiceHandlerTests
{
    [Fact]
    public async Task Success()
    {
        var invoice = InvoiceBuilder.Build();

        var result = await CreateHandler(invoice).Handle(invoice.Id);

        result.Status.ShouldBe("Cancelled");
    }

    [Fact]
    public async Task Handle_ShouldCancel_WhenInvoiceIsOverdue()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.MarkAsOverdue(invoice.DueDate.AddDays(1));

        var result = await CreateHandler(invoice).Handle(invoice.Id);

        result.Status.ShouldBe("Cancelled");
    }

    [Fact]
    public async Task Handle_ShouldThrowConflict_WhenInvoiceIsAlreadyPaid()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Pay(new DateTime(2026, 9, 15));

        var exception = await Should.ThrowAsync<ConflictException>(() => CreateHandler(invoice).Handle(invoice.Id));

        exception.GetStatusCode().ShouldBe(HttpStatusCode.Conflict);
        exception.GetErrorMessages().ShouldContain("A fatura não está em um estado válido para ser cancelada.");
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenInvoiceDoesNotExist()
    {
        var exception = await Should.ThrowAsync<NotFoundException>(() => CreateHandler(null).Handle(Guid.NewGuid()));

        exception.GetErrorMessages().ShouldContain("Fatura não encontrada.");
    }

    private static CancelInvoiceHandler CreateHandler(Invoice? invoice)
    {
        var invoiceBuilder = new IInvoiceRepositoryBuilder();
        if (invoice is not null)
        {
            invoiceBuilder.GetById(invoice);
        }

        return new CancelInvoiceHandler(invoiceBuilder.Build(), IUnitOfWorkBuilder.Build());
    }
}
