using FluentValidation;
using SubscriptionManager.Application.DTOs.Contracts;

namespace SubscriptionManager.Application.Validators;

public class CancelContractRequestValidator : AbstractValidator<CancelContractRequest>
{
    public CancelContractRequestValidator()
    {
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("A data de encerramento é obrigatória.");
    }
}
