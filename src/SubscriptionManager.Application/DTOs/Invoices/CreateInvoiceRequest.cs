namespace SubscriptionManager.Application.DTOs.Invoices;

public record CreateInvoiceRequest(Guid ContractId, decimal Amount, DateTime DueDate, DateTime ReferenceMonth);
