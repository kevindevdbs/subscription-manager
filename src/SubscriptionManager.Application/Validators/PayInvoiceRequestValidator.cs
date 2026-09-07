using FluentValidation;
using SubscriptionManager.Application.DTOs.Invoices;

namespace SubscriptionManager.Application.Validators;

public class PayInvoiceRequestValidator : AbstractValidator<PayInvoiceRequest>
{
    public PayInvoiceRequestValidator()
    {
        // Omitir a data é válido (vale a data corrente); mandar uma data vazia não.
        RuleFor(x => x.PaidAt)
            .Must(value => value!.Value != default).WithMessage("A data do pagamento é inválida.")
            .When(x => x.PaidAt.HasValue);
    }
}
