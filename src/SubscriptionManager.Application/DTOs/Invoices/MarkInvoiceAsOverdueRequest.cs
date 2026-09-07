namespace SubscriptionManager.Application.DTOs.Invoices;

/// <summary>
/// <paramref name="ReferenceDate"/> é opcional: quando omitido o vencimento é
/// apurado contra a data corrente.
/// </summary>
public record MarkInvoiceAsOverdueRequest(DateTime? ReferenceDate = null);
