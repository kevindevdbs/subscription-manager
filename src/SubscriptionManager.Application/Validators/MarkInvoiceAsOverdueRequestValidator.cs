using FluentValidation;
using SubscriptionManager.Application.DTOs.Invoices;

namespace SubscriptionManager.Application.Validators;

public class MarkInvoiceAsOverdueRequestValidator : AbstractValidator<MarkInvoiceAsOverdueRequest>
{
    public MarkInvoiceAsOverdueRequestValidator()
    {
        RuleFor(x => x.ReferenceDate)
            .NotEmpty().WithMessage("A data de referência é obrigatória.");
    }
}
