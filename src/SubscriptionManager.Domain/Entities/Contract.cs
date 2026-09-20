using SubscriptionManager.Domain.Enums;
using SubscriptionManager.Domain.Exceptions;

namespace SubscriptionManager.Domain.Entities;

public class Contract
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid PlanId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; } = null!;
    public ContractStatus Status { get; private set; }


    private Contract()
    {

    }

    public Contract(Guid customerId, Guid planId, DateTime startDate)
    {

        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer ID não pode ser vazio", nameof(customerId));

        if (planId == Guid.Empty)
            throw new ArgumentException("Plan ID não pode ser vazio", nameof(planId));

        Id = Guid.NewGuid();
        CustomerId = customerId;
        PlanId = planId;
        StartDate = startDate;
        Status = ContractStatus.Active;
    }

    /// <summary>
    /// O mês da assinatura não é cobrado: a primeira fatura sai no mês seguinte,
    /// para o cliente pagar um mês depois de contratar.
    /// </summary>
    public bool IsBillableIn(DateTime referenceMonth)
    {
        var firstDayOfMonth = new DateTime(referenceMonth.Year, referenceMonth.Month, 1);

        return StartDate < firstDayOfMonth;
    }

    /// <summary>
    /// A fatura vence no mesmo dia do mês em que o contrato foi assinado.
    /// </summary>
    public DateTime DueDateFor(DateTime referenceMonth)
    {
        var daysInMonth = DateTime.DaysInMonth(referenceMonth.Year, referenceMonth.Month);

        // Contrato assinado no dia 31 vence no último dia dos meses mais curtos, em
        // vez de escorregar para o mês seguinte.
        var dueDay = Math.Min(StartDate.Day, daysInMonth);

        return new DateTime(referenceMonth.Year, referenceMonth.Month, dueDay);
    }

    public void Suspend()
    {
        if (Status != ContractStatus.Active)
        {
            throw new ConflictException("O contrato não está ativo e não pode ser suspenso.");
        }
        Status = ContractStatus.Suspended;
    }

    public void Cancel(DateTime endDate)
    {
        if (Status == ContractStatus.Cancelled)
        {
            throw new ConflictException("O contrato já está cancelado.");
        }
        Status = ContractStatus.Cancelled;
        EndDate = endDate;
    }

    public void Reactivate()
    {
        if (Status != ContractStatus.Suspended)
        {
            throw new ConflictException("O contrato não está suspenso e não pode ser ativado.");
        }
        Status = ContractStatus.Active;
    }
}
