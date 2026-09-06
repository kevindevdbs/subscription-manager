using FluentValidation.Results;
using SubscriptionManager.Application.DTOs.Plans;
using SubscriptionManager.Application.Validators;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Exceptions;
using SubscriptionManager.Domain.Repositories;
using SubscriptionManager.Domain.ValueObjects;

namespace SubscriptionManager.Application.UseCases.Plans;

public class CreatePlanHandler
{

    private readonly IPlanRepository _planRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePlanHandler(IPlanRepository planRepository, IUnitOfWork unitOfWork)
    {
        _planRepository = planRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PlanResponse> Handle(CreatePlanRequest request)
    {
        await ValidateAndThrowOnFailures(request);

        var plan = new Plan(request.Name, new Money(request.MonthlyPrice));
        await _planRepository.AddAsync(plan);
        await _unitOfWork.SaveChangesAsync();

        return new PlanResponse(plan.Id, plan.Name, plan.MonthlyPrice.Amount, plan.IsActive);
    }

    private async Task ValidateAndThrowOnFailures(CreatePlanRequest request)
    {
        var validator = new CreatePlanRequestValidator();

        var result = validator.Validate(request);

        var nameExists = await _planRepository.ExistsActiveWithNameAsync(request.Name);
        if (nameExists)
        {
            result.Errors.Add(new ValidationFailure(string.Empty, "Já existe um plano ativo com esse nome."));
        }

        if (result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
