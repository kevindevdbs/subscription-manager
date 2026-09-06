using FluentValidation;
using SubscriptionManager.Application.DTOs.Invoices;

namespace SubscriptionManager.Application.Validators;

public class GenerateInvoiceRequestValidator : AbstractValidator<GenerateInvoiceRequest>
{
    public GenerateInvoiceRequestValidator()
    {
        RuleFor(x => x.ReferenceMonth)
            .NotEmpty().WithMessage("O mês de referência é obrigatório.");
    }
}
