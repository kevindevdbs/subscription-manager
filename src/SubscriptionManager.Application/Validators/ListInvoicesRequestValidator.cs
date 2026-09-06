using FluentValidation;
using SubscriptionManager.Application.DTOs.Invoices;
using SubscriptionManager.Domain.Enums;

namespace SubscriptionManager.Application.Validators;

public class ListInvoicesRequestValidator : AbstractValidator<ListInvoicesRequest>
{
    public ListInvoicesRequestValidator()
    {
        RuleFor(x => x.Status)
            .Custom((value, context) =>
            {
                if (!context.InstanceToValidate.TryGetStatus(out _))
                {
                    context.AddFailure(
                        nameof(ListInvoicesRequest.Status),
                        $"Status inválido. Valores aceitos: {string.Join(", ", Enum.GetNames<InvoiceStatus>())}.");
                }
            });

        RuleFor(x => x.Month)
            .Custom((value, context) =>
            {
                if (!context.InstanceToValidate.TryGetMonth(out _))
                {
                    context.AddFailure(
                        nameof(ListInvoicesRequest.Month),
                        $"Mês inválido. Use o formato {ListInvoicesRequest.MonthFormat} (ex.: 2026-09).");
                }
            });
    }
}
