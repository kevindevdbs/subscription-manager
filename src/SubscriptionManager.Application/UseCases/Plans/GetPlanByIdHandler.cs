using SubscriptionManager.Application.DTOs.Plans;
using SubscriptionManager.Domain.Exceptions;
using SubscriptionManager.Domain.Repositories;

namespace SubscriptionManager.Application.UseCases.Plans;

public class GetPlanByIdHandler
{
    private readonly IPlanRepository _planRepository;

    public GetPlanByIdHandler(IPlanRepository planRepository)
    {
        _planRepository = planRepository;
    }

    public async Task<PlanResponse> Handle(Guid id)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        if (plan is null)
        {
            throw new NotFoundException("Plano não encontrado.");
        }

        return new PlanResponse(plan.Id, plan.Name, plan.MonthlyPrice.Amount, plan.IsActive);
    }
}
