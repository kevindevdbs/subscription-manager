namespace SubscriptionManager.Application.DTOs.Invoices;

public record InvoiceResponse(Guid Id, Guid ContractId, decimal Amount, DateTime DueDate, DateTime ReferenceMonth, DateTime? PaidAt, string Status);