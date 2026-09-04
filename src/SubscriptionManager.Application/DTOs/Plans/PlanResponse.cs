namespace SubscriptionManager.Application.DTOs.Plans;

public record PlanResponse(Guid Id, string Name, decimal MonthlyPrice, bool IsActive);
