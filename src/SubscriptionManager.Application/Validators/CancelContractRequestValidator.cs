using FluentValidation;
using SubscriptionManager.Application.DTOs.Contracts;

namespace SubscriptionManager.Application.Validators;

public class CancelContractRequestValidator : AbstractValidator<CancelContractRequest>
{
    public CancelContractRequestValidator()
    {
        // Omitir a data é válido (vale a data corrente); mandar uma data vazia não.
        RuleFor(x => x.EndDate)
            .Must(value => value!.Value != default).WithMessage("A data de encerramento é inválida.")
            .When(x => x.EndDate.HasValue);
    }
}
