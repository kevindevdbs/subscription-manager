using Shouldly;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Enums;
using SubscriptionManager.Domain.Tests.Builders;
using SubscriptionManager.Domain.ValueObjects;
using SubscriptionManager.Domain.Exceptions;

namespace SubscriptionManager.Domain.Tests;

public class InvoiceTests
{
    [Fact]
    public void Pay_PendingInvoice_ChangesStatusToPaid()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Pay(DateTime.Now);
        invoice.Status.ShouldBe(InvoiceStatus.Paid);
    }

    [Fact]
    public void Pay_AlreadyPaidInvoice_ThrowsConflictException()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Pay(DateTime.Now);
        Should.Throw<ConflictException>(() => invoice.Pay(DateTime.Now));
    }

    [Fact]
    public void Cancel_PaidInvoice_ThrowsConflictException()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Pay(DateTime.Now);
        Should.Throw<ConflictException>(() => invoice.Cancel());
    }

    [Fact]
    public void MarkAsOverdue_BeforeDueDate_DoesNotChangeStatus()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.MarkAsOverdue(DateTime.Now);
        invoice.Status.ShouldBe(InvoiceStatus.Pending);

    }

    [Fact]
    public void Constructor_MidMonthReferenceMonth_NormalizesToFirstDayOfMonth()
    {
        var invoice = new Invoice(Guid.NewGuid(), new Money(100), DateTime.Now, new DateTime(2026, 3, 17));

        invoice.ReferenceMonth.ShouldBe(new DateTime(2026, 3, 1));
    }

    [Fact]
    public void Constructor_ReferenceMonthWithTime_RemovesTimeComponent()
    {
        var invoice = new Invoice(Guid.NewGuid(), new Money(100), DateTime.Now, new DateTime(2026, 3, 17, 14, 30, 45));

        invoice.ReferenceMonth.ShouldBe(new DateTime(2026, 3, 1));
    }

    [Fact]
    public void Constructor_LastDayOfMonthReferenceMonth_NormalizesToFirstDayOfMonth()
    {
        var invoice = new Invoice(Guid.NewGuid(), new Money(100), DateTime.Now, new DateTime(2026, 1, 31));

        invoice.ReferenceMonth.ShouldBe(new DateTime(2026, 1, 1));
    }

    [Fact]
    public void Constructor_FirstDayOfMonthReferenceMonth_KeepsDate()
    {
        var invoice = new Invoice(Guid.NewGuid(), new Money(100), DateTime.Now, new DateTime(2026, 3, 1));

        invoice.ReferenceMonth.ShouldBe(new DateTime(2026, 3, 1));
    }

    [Fact]
    public void Constructor_ValidData_SetsStatusToPending()
    {
        var invoice = InvoiceBuilder.Build();

        invoice.Status.ShouldBe(InvoiceStatus.Pending);
    }

    [Fact]
    public void Constructor_EmptyContractId_ThrowsArgumentException()
    {
        Should.Throw<ArgumentException>(() => new Invoice(Guid.Empty, new Money(100), DateTime.Now, DateTime.Now));
    }

    [Fact]
    public void Constructor_NullAmount_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => new Invoice(Guid.NewGuid(), null!, DateTime.Now, DateTime.Now));
    }

    [Fact]
    public void Constructor_ZeroAmount_ThrowsArgumentException()
    {
        Should.Throw<ArgumentException>(() => new Invoice(Guid.NewGuid(), new Money(0), DateTime.Now, DateTime.Now));
    }

    [Fact]
    public void Pay_PendingInvoice_SetsPaidAt()
    {
        var invoice = InvoiceBuilder.Build();
        var paidAt = DateTime.Now;

        invoice.Pay(paidAt);

        invoice.PaidAt.ShouldBe(paidAt);
    }

    [Fact]
    public void Pay_OverdueInvoice_ChangesStatusToPaid()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.MarkAsOverdue(invoice.DueDate.AddDays(1));

        invoice.Pay(DateTime.Now);

        invoice.Status.ShouldBe(InvoiceStatus.Paid);
    }

    [Fact]
    public void Pay_CancelledInvoice_ThrowsConflictException()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Cancel();

        Should.Throw<ConflictException>(() => invoice.Pay(DateTime.Now));
    }

    [Fact]
    public void Pay_RefundedInvoice_ThrowsConflictException()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Pay(DateTime.Now);
        invoice.Refund();

        Should.Throw<ConflictException>(() => invoice.Pay(DateTime.Now));
    }

    [Fact]
    public void MarkAsOverdue_AfterDueDate_ChangesStatusToOverdue()
    {
        var invoice = InvoiceBuilder.Build();

        invoice.MarkAsOverdue(invoice.DueDate.AddDays(1));

        invoice.Status.ShouldBe(InvoiceStatus.Overdue);
    }

    [Fact]
    public void MarkAsOverdue_OnDueDate_DoesNotChangeStatus()
    {
        var invoice = InvoiceBuilder.Build();

        invoice.MarkAsOverdue(invoice.DueDate);

        invoice.Status.ShouldBe(InvoiceStatus.Pending);
    }

    [Fact]
    public void MarkAsOverdue_PaidInvoice_ThrowsConflictException()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Pay(DateTime.Now);

        Should.Throw<ConflictException>(() => invoice.MarkAsOverdue(invoice.DueDate.AddDays(1)));
    }

    [Fact]
    public void MarkAsOverdue_CancelledInvoice_ThrowsConflictException()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Cancel();

        Should.Throw<ConflictException>(() => invoice.MarkAsOverdue(invoice.DueDate.AddDays(1)));
    }

    [Fact]
    public void Refund_PaidInvoice_ChangesStatusToRefunded()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Pay(DateTime.Now);

        invoice.Refund();

        invoice.Status.ShouldBe(InvoiceStatus.Refunded);
    }

    [Fact]
    public void Refund_PendingInvoice_ThrowsConflictException()
    {
        var invoice = InvoiceBuilder.Build();

        Should.Throw<ConflictException>(() => invoice.Refund());
    }

    [Fact]
    public void Refund_CancelledInvoice_ThrowsConflictException()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Cancel();

        Should.Throw<ConflictException>(() => invoice.Refund());
    }

    [Fact]
    public void Refund_RefundedInvoice_ThrowsConflictException()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Pay(DateTime.Now);
        invoice.Refund();

        Should.Throw<ConflictException>(() => invoice.Refund());
    }

    [Fact]
    public void Cancel_PendingInvoice_ChangesStatusToCancelled()
    {
        var invoice = InvoiceBuilder.Build();

        invoice.Cancel();

        invoice.Status.ShouldBe(InvoiceStatus.Cancelled);
    }

    [Fact]
    public void Cancel_OverdueInvoice_ChangesStatusToCancelled()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.MarkAsOverdue(invoice.DueDate.AddDays(1));

        invoice.Cancel();

        invoice.Status.ShouldBe(InvoiceStatus.Cancelled);
    }

    [Fact]
    public void Cancel_CancelledInvoice_ThrowsConflictException()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Cancel();

        Should.Throw<ConflictException>(() => invoice.Cancel());
    }

    [Fact]
    public void Cancel_RefundedInvoice_ThrowsConflictException()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Pay(DateTime.Now);
        invoice.Refund();

        Should.Throw<ConflictException>(() => invoice.Cancel());
    }
}
