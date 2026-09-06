using SubscriptionManager.Application.DTOs.Plans;
using SubscriptionManager.Domain.Repositories;

namespace SubscriptionManager.Application.UseCases.Plans;

public class ListPlansHandler
{
    private readonly IPlanRepository _planRepository;

    public ListPlansHandler(IPlanRepository planRepository)
    {
        _planRepository = planRepository;
    }

    public async Task<IReadOnlyList<PlanResponse>> Handle()
    {
        var plans = await _planRepository.GetAllAsync();

        return plans
            .Select(plan => new PlanResponse(plan.Id, plan.Name, plan.MonthlyPrice.Amount, plan.IsActive))
            .ToList();
    }
}
