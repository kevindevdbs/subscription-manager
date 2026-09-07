namespace SubscriptionManager.Application.DTOs.Invoices;

/// <summary>
/// <paramref name="PaidAt"/> é opcional: quando omitido o pagamento é
/// registrado com a data corrente.
/// </summary>
public record PayInvoiceRequest(DateTime? PaidAt = null);
