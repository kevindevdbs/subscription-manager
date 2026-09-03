using SubscriptionManager.Domain.Enums;
using SubscriptionManager.Domain.ValueObjects;

using SubscriptionManager.Domain.Resources;

namespace SubscriptionManager.Domain.Entities;

public class Invoice
{
    public Guid Id { get; private set; }
    public Guid ContractId { get; private set; }
    public Money Amount { get; private set; } = null!;
    public DateTime DueDate { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public DateTime ReferenceMonth { get; private set; }
    public InvoiceStatus Status { get; private set; } = InvoiceStatus.Pending;

    private Invoice() { }

    public Invoice(Guid contractId, Money amount, DateTime dueDate, DateTime referenceMonth)
    {
        if (contractId == Guid.Empty)
        {
            throw new ArgumentException(DomainMessages.InvoiceContractIdRequired, nameof(contractId));
        }

        if (amount == null)
        {
            throw new ArgumentNullException(nameof(amount), DomainMessages.InvoiceAmountRequired);
        }

        if (amount.Amount <= 0)
        {
            throw new ArgumentException(DomainMessages.InvoiceAmountMustBeGreaterThanZero, nameof(amount));
        }

        Id = Guid.NewGuid();
        Status = InvoiceStatus.Pending;
        ContractId = contractId;
        Amount = amount;
        DueDate = dueDate;
        ReferenceMonth = new DateTime(referenceMonth.Year, referenceMonth.Month, 1);
    }


    public void Pay(DateTime paidAt)
    {

        if (this.Status != InvoiceStatus.Pending && this.Status != InvoiceStatus.Overdue)
        {
            throw new InvalidOperationException(DomainMessages.InvoiceCannotBePaid);
        }

        PaidAt = paidAt;
        Status = InvoiceStatus.Paid;

    }

    public void MarkAsOverdue(DateTime referenceDate)
    {
        if (this.Status != InvoiceStatus.Pending)
        {
            throw new InvalidOperationException(DomainMessages.InvoiceCannotBeMarkedAsOverdue);
        }

        if (referenceDate > this.DueDate)
        {
            this.Status = InvoiceStatus.Overdue;
        }
    }

    public void Refund()
    {
        if (this.Status != InvoiceStatus.Paid)
        {
            throw new InvalidOperationException(DomainMessages.InvoiceCannotBeRefunded);
        }
        this.Status = InvoiceStatus.Refunded;
    }

    public void Cancel()
    {
        if (this.Status != InvoiceStatus.Pending && this.Status != InvoiceStatus.Overdue)
        {
            throw new InvalidOperationException(DomainMessages.InvoiceCannotBeCancelled);
        }
        this.Status = InvoiceStatus.Cancelled;
    }
}
