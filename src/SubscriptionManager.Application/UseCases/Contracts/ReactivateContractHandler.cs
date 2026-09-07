using SubscriptionManager.Application.DTOs.Contracts;
using SubscriptionManager.Application.Mappers;
using SubscriptionManager.Domain.Exceptions;
using SubscriptionManager.Domain.Repositories;

namespace SubscriptionManager.Application.UseCases.Contracts;

public class ReactivateContractHandler
{
    private readonly IContractRepository _contractRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReactivateContractHandler(IContractRepository contractRepository, IUnitOfWork unitOfWork)
    {
        _contractRepository = contractRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ContractResponse> Handle(Guid id)
    {
        var contract = await _contractRepository.GetByIdAsync(id);
        if (contract is null)
        {
            throw new NotFoundException("Contrato não encontrado.");
        }

        contract.Reactivate();

        await _unitOfWork.SaveChangesAsync();

        return contract.ToResponse();
    }
}
