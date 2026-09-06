using SubscriptionManager.Application.DTOs.Contracts;
using SubscriptionManager.Domain.Exceptions;
using SubscriptionManager.Domain.Repositories;

namespace SubscriptionManager.Application.UseCases.Contracts;

public class GetContractByIdHandler
{
    private readonly IContractRepository _contractRepository;

    public GetContractByIdHandler(IContractRepository contractRepository)
    {
        _contractRepository = contractRepository;
    }

    public async Task<ContractResponse> Handle(Guid id)
    {
        var contract = await _contractRepository.GetByIdAsync(id);
        if (contract is null)
        {
            throw new NotFoundException("Contrato não encontrado.");
        }

        return new ContractResponse(
            contract.Id,
            contract.CustomerId,
            contract.PlanId,
            contract.StartDate,
            contract.EndDate,
            contract.Status.ToString());
    }
}
