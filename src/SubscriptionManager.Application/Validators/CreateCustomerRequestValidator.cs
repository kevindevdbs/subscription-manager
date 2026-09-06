using FluentValidation;
using SubscriptionManager.Application.DTOs.Customers;

namespace SubscriptionManager.Application.Validators;

public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("O e-mail informado não é válido.")
            .MaximumLength(100).WithMessage("O e-mail deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Document)
            .NotEmpty().WithMessage("O documento é obrigatório.")
            .MaximumLength(20).WithMessage("O documento deve ter no máximo 20 caracteres.");
    }
}
