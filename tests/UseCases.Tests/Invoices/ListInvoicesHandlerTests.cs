using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using Shouldly;
using SubscriptionManager.Application.DTOs.Invoices;
using SubscriptionManager.Application.UseCases.Invoices;
using SubscriptionManager.Domain.Exceptions;

namespace UseCases.Tests.Invoices;

public class ListInvoicesHandlerTests
{
    [Fact]
    public async Task Success_WithoutFilters()
    {
        var invoice = InvoiceBuilder.Build(amount: 150);

        var handler = new ListInvoicesHandler(new IInvoiceRepositoryBuilder().GetFiltered(invoice).Build());

        var result = await handler.Handle(new ListInvoicesRequest(null, null));

        result.Count.ShouldBe(1);
        result[0].Amount.ShouldBe(150);
    }

    [Fact]
    public async Task Success_WithStatusAndMonthFilters()
    {
        var invoice = InvoiceBuilder.Build();

        var handler = new ListInvoicesHandler(new IInvoiceRepositoryBuilder().GetFiltered(invoice).Build());

        var result = await handler.Handle(new ListInvoicesRequest("Pending", "2026-09"));

        result.Count.ShouldBe(1);
        result[0].Status.ShouldBe("Pending");
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenStatusIsInvalid()
    {
        var handler = new ListInvoicesHandler(new IInvoiceRepositoryBuilder().Build());

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(() => handler.Handle(new ListInvoicesRequest("xpto", null)));

        exception.GetErrorMessages().ShouldContain(message => message.StartsWith("Status inválido."));
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenMonthFormatIsInvalid()
    {
        var handler = new ListInvoicesHandler(new IInvoiceRepositoryBuilder().Build());

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(() => handler.Handle(new ListInvoicesRequest(null, "setembro")));

        exception.GetErrorMessages().ShouldContain(message => message.StartsWith("Mês inválido."));
    }
}
