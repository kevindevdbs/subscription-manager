using FluentValidation;
using SubscriptionManager.Application.DTOs.Invoices;

namespace SubscriptionManager.Application.Validators;

public class MarkInvoiceAsOverdueRequestValidator : AbstractValidator<MarkInvoiceAsOverdueRequest>
{
    public MarkInvoiceAsOverdueRequestValidator()
    {
        // Omitir a data é válido (vale a data corrente); mandar uma data vazia não.
        RuleFor(x => x.ReferenceDate)
            .Must(value => value!.Value != default).WithMessage("A data de referência é inválida.")
            .When(x => x.ReferenceDate.HasValue);
    }
}
