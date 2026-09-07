using SubscriptionManager.Application.DTOs.Contracts;
using SubscriptionManager.Application.Mappers;
using SubscriptionManager.Application.Validators;
using SubscriptionManager.Domain.Exceptions;
using SubscriptionManager.Domain.Repositories;

namespace SubscriptionManager.Application.UseCases.Contracts;

public class CancelContractHandler
{
    private readonly IContractRepository _contractRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelContractHandler(IContractRepository contractRepository, IUnitOfWork unitOfWork)
    {
        _contractRepository = contractRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ContractResponse> Handle(Guid id, CancelContractRequest request)
    {
        Validate(request);

        var contract = await _contractRepository.GetByIdAsync(id);
        if (contract is null)
        {
            throw new NotFoundException("Contrato não encontrado.");
        }

        if (request.EndDate < contract.StartDate)
        {
            throw new ErrorOnValidationException(["A data de encerramento não pode ser anterior à data de início."]);
        }

        try
        {
            contract.Cancel(request.EndDate);
        }
        catch (InvalidOperationException exception)
        {
            throw new ConflictException(exception.Message);
        }

        await _unitOfWork.SaveChangesAsync();

        return contract.ToResponse();
    }

    private static void Validate(CancelContractRequest request)
    {
        var validator = new CancelContractRequestValidator();

        var result = validator.Validate(request);

        if (result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
