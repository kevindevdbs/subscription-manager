namespace SubscriptionManager.Application.DTOs.Contracts;

public record ContractResponse(Guid Id, Guid CustomerId, Guid PlanId, DateTime StartDate, DateTime? EndDate, string Status);
