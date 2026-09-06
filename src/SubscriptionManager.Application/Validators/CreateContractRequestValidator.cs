using FluentValidation;
using SubscriptionManager.Application.DTOs.Contracts;

namespace SubscriptionManager.Application.Validators;

public class CreateContractRequestValidator : AbstractValidator<CreateContractRequest>
{
    public CreateContractRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("O identificador do cliente é obrigatório.");

        RuleFor(x => x.PlanId)
            .NotEmpty().WithMessage("O identificador do plano é obrigatório.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("A data de início é obrigatória.");
    }
}
