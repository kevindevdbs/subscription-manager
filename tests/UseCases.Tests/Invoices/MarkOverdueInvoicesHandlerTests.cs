using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using Microsoft.Extensions.Time.Testing;
using Shouldly;
using SubscriptionManager.Application.UseCases.Invoices;
using SubscriptionManager.Domain.Enums;

namespace UseCases.Tests.Invoices;

public class MarkOverdueInvoicesHandlerTests
{
    // InvoiceBuilder gera o vencimento no dia 10 do mês, à meia-noite.
    private static readonly DateTime September = new(2026, 9, 1);

    [Fact]
    public async Task Success()
    {
        var first = InvoiceBuilder.Build(referenceMonth: September);
        var second = InvoiceBuilder.Build(referenceMonth: September.AddMonths(1));

        var invoiceRepository = new IInvoiceRepositoryBuilder().GetPendingDueBefore(first, second);

        var result = await CreateHandler(At(2026, 12, 1), invoiceRepository).Handle();

        result.ShouldBe(2);
        first.Status.ShouldBe(InvoiceStatus.Overdue);
        second.Status.ShouldBe(InvoiceStatus.Overdue);
    }

    [Fact]
    public async Task Handle_ShouldReturnZero_WhenNothingIsDue()
    {
        var invoiceRepository = new IInvoiceRepositoryBuilder().GetPendingDueBefore();

        var result = await CreateHandler(At(2026, 12, 1), invoiceRepository).Handle();

        result.ShouldBe(0);
    }

    [Fact]
    public async Task Handle_ShouldLookForInvoicesDueBeforeTheStartOfToday()
    {
        var invoiceRepository = new IInvoiceRepositoryBuilder().GetPendingDueBefore();

        await CreateHandler(At(2026, 9, 10, hour: 15, minute: 30), invoiceRepository).Handle();

        invoiceRepository.VerifyPendingDueBefore(new DateTime(2026, 9, 10));
    }

    [Fact]
    public async Task Handle_ShouldKeepItPending_UntilTheEndOfItsDueDay()
    {
        var invoice = InvoiceBuilder.Build(referenceMonth: September);

        var invoiceRepository = new IInvoiceRepositoryBuilder().GetPendingDueBefore(invoice);

        await CreateHandler(At(2026, 9, 10, hour: 23, minute: 59), invoiceRepository).Handle();

        invoice.Status.ShouldBe(InvoiceStatus.Pending);
    }

    [Fact]
    public async Task Handle_ShouldMarkItOverdue_OnTheDayAfterItsDueDay()
    {
        var invoice = InvoiceBuilder.Build(referenceMonth: September);

        var invoiceRepository = new IInvoiceRepositoryBuilder().GetPendingDueBefore(invoice);

        await CreateHandler(At(2026, 9, 11, hour: 0, minute: 1), invoiceRepository).Handle();

        invoice.Status.ShouldBe(InvoiceStatus.Overdue);
    }

    private static DateTimeOffset At(int year, int month, int day, int hour = 0, int minute = 0)
    {
        return new DateTimeOffset(year, month, day, hour, minute, 0, TimeSpan.Zero);
    }

    private static MarkOverdueInvoicesHandler CreateHandler(DateTimeOffset now, IInvoiceRepositoryBuilder invoiceRepository)
    {
        return new MarkOverdueInvoicesHandler(invoiceRepository.Build(), IUnitOfWorkBuilder.Build(), new FakeTimeProvider(now));
    }
}
