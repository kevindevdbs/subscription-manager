using SubscriptionManager.Domain.Enums;


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

    public void Suspend()
    {
        if (Status != ContractStatus.Active)
        {
            throw new InvalidOperationException("O contrato não está ativo e não pode ser suspenso.");
        }
        Status = ContractStatus.Suspended;
    }

    public void Cancel(DateTime endDate)
    {
        if (Status == ContractStatus.Cancelled)
        {
            throw new InvalidOperationException("O contrato já está cancelado.");
        }
        Status = ContractStatus.Cancelled;
        EndDate = endDate;
    }

    public void Reactivate()
    {
        if (Status != ContractStatus.Suspended)
        {
            throw new InvalidOperationException("O contrato não está suspenso e não pode ser ativado.");
        }
        Status = ContractStatus.Active;
    }
}
