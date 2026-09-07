using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using Shouldly;
using SubscriptionManager.Application.DTOs.Invoices;
using SubscriptionManager.Application.UseCases.Invoices;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Exceptions;

namespace UseCases.Tests.Invoices;

public class MarkOverdueInvoicesHandlerTests
{
    private static readonly DateTime ReferenceDate = new(2026, 12, 1);

    [Fact]
    public async Task Success()
    {
        var first = InvoiceBuilder.Build(referenceMonth: new DateTime(2026, 9, 1));
        var second = InvoiceBuilder.Build(referenceMonth: new DateTime(2026, 10, 1));

        var request = new MarkInvoiceAsOverdueRequest(ReferenceDate);

        var result = await CreateHandler(first, second).Handle(request);

        result.ShouldBe(2);
        first.Status.ShouldBe(SubscriptionManager.Domain.Enums.InvoiceStatus.Overdue);
        second.Status.ShouldBe(SubscriptionManager.Domain.Enums.InvoiceStatus.Overdue);
    }

    [Fact]
    public async Task Handle_ShouldReturnZero_WhenNothingIsDue()
    {
        var result = await CreateHandler().Handle(new MarkInvoiceAsOverdueRequest(ReferenceDate));

        result.ShouldBe(0);
    }

    [Fact]
    public async Task Handle_ShouldFallBackToNow_WhenReferenceDateIsOmitted()
    {
        var invoice = InvoiceBuilder.Build(referenceMonth: new DateTime(2020, 1, 1));

        var result = await CreateHandler(invoice).Handle(new MarkInvoiceAsOverdueRequest());

        result.ShouldBe(1);
        invoice.Status.ShouldBe(SubscriptionManager.Domain.Enums.InvoiceStatus.Overdue);
    }

    [Fact]
    public async Task Handle_ShouldFallBackToNow_WhenThereIsNoBodyAtAll()
    {
        var invoice = InvoiceBuilder.Build(referenceMonth: new DateTime(2020, 1, 1));

        var result = await CreateHandler(invoice).Handle(null);

        result.ShouldBe(1);
    }

    [Fact]
    public async Task Handle_ShouldValidateBeforeTouchingTheRepository()
    {
        var exception = await Should.ThrowAsync<ErrorOnValidationException>(
            () => CreateHandler().Handle(new MarkInvoiceAsOverdueRequest(default(DateTime))));

        exception.GetErrorMessages().ShouldContain("A data de referência é inválida.");
    }

    private static MarkOverdueInvoicesHandler CreateHandler(params Invoice[] invoices)
    {
        var invoiceBuilder = new IInvoiceRepositoryBuilder().GetPendingDueBefore(invoices);

        return new MarkOverdueInvoicesHandler(invoiceBuilder.Build(), IUnitOfWorkBuilder.Build());
    }
}
