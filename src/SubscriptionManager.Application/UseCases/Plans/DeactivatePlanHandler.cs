using SubscriptionManager.Application.DTOs.Plans;
using SubscriptionManager.Domain.Exceptions;
using SubscriptionManager.Domain.Repositories;

namespace SubscriptionManager.Application.UseCases.Plans;

/// <summary>
/// Descontinua um plano. Os contratos que já apontam para ele seguem ativos e
/// continuam sendo faturados — desativar só fecha a porta para adesão nova.
/// </summary>
public class DeactivatePlanHandler
{
    private readonly IPlanRepository _planRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivatePlanHandler(IPlanRepository planRepository, IUnitOfWork unitOfWork)
    {
        _planRepository = planRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PlanResponse> Handle(Guid id)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        if (plan is null)
        {
            throw new NotFoundException("Plano não encontrado.");
        }

        plan.Deactivate();

        await _unitOfWork.SaveChangesAsync();

        return new PlanResponse(plan.Id, plan.Name, plan.MonthlyPrice.Amount, plan.IsActive);
    }
}
