namespace SubscriptionManager.Application.DTOs.Contracts;

public record CreateContractRequest(Guid CustomerId, Guid PlanId, DateTime StartDate);
