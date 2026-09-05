using FluentValidation;
using SubscriptionManager.Application.DTOs.Plans;

namespace SubscriptionManager.Application.Validators;

public class CreatePlanRequestValidator : AbstractValidator<CreatePlanRequest>
{
    public CreatePlanRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome do plano é obrigatório.")
            .MaximumLength(100).WithMessage("O nome do plano deve ter no máximo 100 caracteres.");

        RuleFor(x => x.MonthlyPrice)
            .GreaterThan(0).WithMessage("O preço mensal deve ser maior que zero.");
    }
}
