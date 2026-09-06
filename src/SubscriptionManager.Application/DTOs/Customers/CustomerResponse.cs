namespace SubscriptionManager.Application.DTOs.Customers;

public record CustomerResponse(Guid Id, string Name, string Email, string Document, DateTime CreatedAt);

