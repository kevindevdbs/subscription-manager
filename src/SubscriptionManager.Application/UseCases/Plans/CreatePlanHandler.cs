using SubscriptionManager.Application.DTOs.Plans;
using SubscriptionManager.Domain.Entities;
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
        var plan = new Plan(request.Name, new Money(request.MonthlyPrice));
        await _planRepository.AddAsync(plan);
        await _unitOfWork.SaveChangesAsync();

        return new PlanResponse(plan.Id, plan.Name, plan.MonthlyPrice.Amount, plan.IsActive);
    }
}
