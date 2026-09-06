using SubscriptionManager.Domain.Enums;
using SubscriptionManager.Domain.ValueObjects;


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
            throw new ArgumentException("O identificador do contrato não pode ser vazio.", nameof(contractId));
        }

        if (amount == null)
        {
            throw new ArgumentNullException(nameof(amount), "O valor da fatura não pode ser nulo.");
        }

        if (amount.Amount <= 0)
        {
            throw new ArgumentException("O valor da fatura não pode ser menor ou igual a zero.", nameof(amount));
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
            throw new InvalidOperationException("A fatura não está em um estado válido para ser paga.");
        }

        PaidAt = paidAt;
        Status = InvoiceStatus.Paid;

    }

    public void MarkAsOverdue(DateTime referenceDate)
    {
        if (this.Status != InvoiceStatus.Pending)
        {
            throw new InvalidOperationException("A fatura não está em um estado válido para ser marcada como vencida.");
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
            throw new InvalidOperationException("A fatura não está em um estado válido para ser reembolsada.");
        }
        this.Status = InvoiceStatus.Refunded;
    }

    public void Cancel()
    {
        if (this.Status != InvoiceStatus.Pending && this.Status != InvoiceStatus.Overdue)
        {
            throw new InvalidOperationException("A fatura não está em um estado válido para ser cancelada.");
        }
        this.Status = InvoiceStatus.Cancelled;
    }
}
