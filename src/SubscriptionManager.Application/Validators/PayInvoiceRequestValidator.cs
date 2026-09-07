using FluentValidation;
using SubscriptionManager.Application.DTOs.Invoices;

namespace SubscriptionManager.Application.Validators;

public class PayInvoiceRequestValidator : AbstractValidator<PayInvoiceRequest>
{
    public PayInvoiceRequestValidator()
    {
        RuleFor(x => x.PaidAt)
            .NotEmpty().WithMessage("A data do pagamento é obrigatória.");
    }
}
