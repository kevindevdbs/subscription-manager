using SubscriptionManager.Application.DTOs.Contracts;
using SubscriptionManager.Domain.Entities;

namespace SubscriptionManager.Application.Mappers;

public static class ContractMapper
{
    public static ContractResponse ToResponse(this Contract contract)
    {
        return new ContractResponse(
            contract.Id,
            contract.CustomerId,
            contract.PlanId,
            contract.StartDate,
            contract.EndDate,
            contract.Status.ToString());
    }

    public static IReadOnlyList<ContractResponse> ToResponse(this IEnumerable<Contract> contracts)
    {
        return contracts.Select(ToResponse).ToList();
    }
}
