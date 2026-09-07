using SubscriptionManager.Application.DTOs.Contracts;
using SubscriptionManager.Application.Mappers;
using SubscriptionManager.Domain.Repositories;

namespace SubscriptionManager.Application.UseCases.Contracts;

public class ListContractsHandler
{
    private readonly IContractRepository _contractRepository;

    public ListContractsHandler(IContractRepository contractRepository)
    {
        _contractRepository = contractRepository;
    }

    public async Task<IReadOnlyList<ContractResponse>> Handle()
    {
        var contracts = await _contractRepository.GetAllAsync();

        return contracts.ToResponse();
    }
}
