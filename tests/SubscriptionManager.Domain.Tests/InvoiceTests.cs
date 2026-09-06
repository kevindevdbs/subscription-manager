using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Enums;
using SubscriptionManager.Domain.Tests.Builders;
using SubscriptionManager.Domain.ValueObjects;

namespace SubscriptionManager.Domain.Tests;

public class InvoiceTests
{
    [Fact]
    public void Pay_PendingInvoice_ChangesStatusToPaid()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Pay(DateTime.Now);
        Assert.Equal(InvoiceStatus.Paid, invoice.Status);
    }

    [Fact]
    public void Pay_AlreadyPaidInvoice_ThrowsInvalidOperationException()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Pay(DateTime.Now);
        Assert.Throws<InvalidOperationException>(() => invoice.Pay(DateTime.Now));
    }

    [Fact]
    public void Cancel_PaidInvoice_ThrowsInvalidOperationException()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Pay(DateTime.Now);
        Assert.Throws<InvalidOperationException>(() => invoice.Cancel());
    }

    [Fact]
    public void MarkAsOverdue_BeforeDueDate_DoesNotChangeStatus()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.MarkAsOverdue(DateTime.Now);
        Assert.Equal(InvoiceStatus.Pending, invoice.Status);

    }

    [Fact]
    public void Constructor_MidMonthReferenceMonth_NormalizesToFirstDayOfMonth()
    {
        var invoice = new Invoice(Guid.NewGuid(), new Money(100), DateTime.Now, new DateTime(2026, 3, 17));

        Assert.Equal(new DateTime(2026, 3, 1), invoice.ReferenceMonth);
    }

    [Fact]
    public void Constructor_ReferenceMonthWithTime_RemovesTimeComponent()
    {
        var invoice = new Invoice(Guid.NewGuid(), new Money(100), DateTime.Now, new DateTime(2026, 3, 17, 14, 30, 45));

        Assert.Equal(new DateTime(2026, 3, 1), invoice.ReferenceMonth);
    }

    [Fact]
    public void Constructor_LastDayOfMonthReferenceMonth_NormalizesToFirstDayOfMonth()
    {
        var invoice = new Invoice(Guid.NewGuid(), new Money(100), DateTime.Now, new DateTime(2026, 1, 31));

        Assert.Equal(new DateTime(2026, 1, 1), invoice.ReferenceMonth);
    }

    [Fact]
    public void Constructor_FirstDayOfMonthReferenceMonth_KeepsDate()
    {
        var invoice = new Invoice(Guid.NewGuid(), new Money(100), DateTime.Now, new DateTime(2026, 3, 1));

        Assert.Equal(new DateTime(2026, 3, 1), invoice.ReferenceMonth);
    }

    [Fact]
    public void Constructor_ValidData_SetsStatusToPending()
    {
        var invoice = InvoiceBuilder.Build();

        Assert.Equal(InvoiceStatus.Pending, invoice.Status);
    }

    [Fact]
    public void Constructor_EmptyContractId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Invoice(Guid.Empty, new Money(100), DateTime.Now, DateTime.Now));
    }

    [Fact]
    public void Constructor_NullAmount_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Invoice(Guid.NewGuid(), null!, DateTime.Now, DateTime.Now));
    }

    [Fact]
    public void Constructor_ZeroAmount_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Invoice(Guid.NewGuid(), new Money(0), DateTime.Now, DateTime.Now));
    }

    [Fact]
    public void Pay_PendingInvoice_SetsPaidAt()
    {
        var invoice = InvoiceBuilder.Build();
        var paidAt = DateTime.Now;

        invoice.Pay(paidAt);

        Assert.Equal(paidAt, invoice.PaidAt);
    }

    [Fact]
    public void Pay_OverdueInvoice_ChangesStatusToPaid()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.MarkAsOverdue(invoice.DueDate.AddDays(1));

        invoice.Pay(DateTime.Now);

        Assert.Equal(InvoiceStatus.Paid, invoice.Status);
    }

    [Fact]
    public void Pay_CancelledInvoice_ThrowsInvalidOperationException()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Cancel();

        Assert.Throws<InvalidOperationException>(() => invoice.Pay(DateTime.Now));
    }

    [Fact]
    public void Pay_RefundedInvoice_ThrowsInvalidOperationException()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Pay(DateTime.Now);
        invoice.Refund();

        Assert.Throws<InvalidOperationException>(() => invoice.Pay(DateTime.Now));
    }

    [Fact]
    public void MarkAsOverdue_AfterDueDate_ChangesStatusToOverdue()
    {
        var invoice = InvoiceBuilder.Build();

        invoice.MarkAsOverdue(invoice.DueDate.AddDays(1));

        Assert.Equal(InvoiceStatus.Overdue, invoice.Status);
    }

    [Fact]
    public void MarkAsOverdue_OnDueDate_DoesNotChangeStatus()
    {
        var invoice = InvoiceBuilder.Build();

        invoice.MarkAsOverdue(invoice.DueDate);

        Assert.Equal(InvoiceStatus.Pending, invoice.Status);
    }

    [Fact]
    public void MarkAsOverdue_PaidInvoice_ThrowsInvalidOperationException()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Pay(DateTime.Now);

        Assert.Throws<InvalidOperationException>(() => invoice.MarkAsOverdue(invoice.DueDate.AddDays(1)));
    }

    [Fact]
    public void MarkAsOverdue_CancelledInvoice_ThrowsInvalidOperationException()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Cancel();

        Assert.Throws<InvalidOperationException>(() => invoice.MarkAsOverdue(invoice.DueDate.AddDays(1)));
    }

    [Fact]
    public void Refund_PaidInvoice_ChangesStatusToRefunded()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Pay(DateTime.Now);

        invoice.Refund();

        Assert.Equal(InvoiceStatus.Refunded, invoice.Status);
    }

    [Fact]
    public void Refund_PendingInvoice_ThrowsInvalidOperationException()
    {
        var invoice = InvoiceBuilder.Build();

        Assert.Throws<InvalidOperationException>(() => invoice.Refund());
    }

    [Fact]
    public void Refund_CancelledInvoice_ThrowsInvalidOperationException()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Cancel();

        Assert.Throws<InvalidOperationException>(() => invoice.Refund());
    }

    [Fact]
    public void Refund_RefundedInvoice_ThrowsInvalidOperationException()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Pay(DateTime.Now);
        invoice.Refund();

        Assert.Throws<InvalidOperationException>(() => invoice.Refund());
    }

    [Fact]
    public void Cancel_PendingInvoice_ChangesStatusToCancelled()
    {
        var invoice = InvoiceBuilder.Build();

        invoice.Cancel();

        Assert.Equal(InvoiceStatus.Cancelled, invoice.Status);
    }

    [Fact]
    public void Cancel_OverdueInvoice_ChangesStatusToCancelled()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.MarkAsOverdue(invoice.DueDate.AddDays(1));

        invoice.Cancel();

        Assert.Equal(InvoiceStatus.Cancelled, invoice.Status);
    }

    [Fact]
    public void Cancel_CancelledInvoice_ThrowsInvalidOperationException()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Cancel();

        Assert.Throws<InvalidOperationException>(() => invoice.Cancel());
    }

    [Fact]
    public void Cancel_RefundedInvoice_ThrowsInvalidOperationException()
    {
        var invoice = InvoiceBuilder.Build();
        invoice.Pay(DateTime.Now);
        invoice.Refund();

        Assert.Throws<InvalidOperationException>(() => invoice.Cancel());
    }
}
