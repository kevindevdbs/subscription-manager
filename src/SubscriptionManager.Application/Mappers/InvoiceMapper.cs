using SubscriptionManager.Application.DTOs.Invoices;
using SubscriptionManager.Domain.Entities;

namespace SubscriptionManager.Application.Mappers;

public static class InvoiceMapper
{
    public static InvoiceResponse ToResponse(this Invoice invoice)
    {
        return new InvoiceResponse(
            invoice.Id,
            invoice.ContractId,
            invoice.Amount.Amount,
            invoice.DueDate,
            invoice.ReferenceMonth,
            invoice.PaidAt,
            invoice.Status.ToString());
    }

    public static IReadOnlyList<InvoiceResponse> ToResponse(this IEnumerable<Invoice> invoices)
    {
        return invoices.Select(ToResponse).ToList();
    }
}
