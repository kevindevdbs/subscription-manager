using SubscriptionManager.Domain.Enums;
using SubscriptionManager.Domain.Tests.Builders;

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
}
