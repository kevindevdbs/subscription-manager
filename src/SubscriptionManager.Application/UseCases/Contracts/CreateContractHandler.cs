using SubscriptionManager.Application.DTOs.Contracts;
using SubscriptionManager.Application.Validators;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Exceptions;
using SubscriptionManager.Domain.Repositories;

namespace SubscriptionManager.Application.UseCases.Contracts;

public class CreateContractHandler
{

    private readonly IContractRepository _contractRepository;

    private readonly ICustomerRepository _customerRepository;

    private readonly IPlanRepository _planRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateContractHandler(IContractRepository contractRepository, IUnitOfWork unitOfWork, ICustomerRepository customerRepository, IPlanRepository planRepository)
    {
        _contractRepository = contractRepository;
        _unitOfWork = unitOfWork;
        _customerRepository = customerRepository;
        _planRepository = planRepository;
    }

    public async Task<ContractResponse> Handle(CreateContractRequest request)
    {
        Validate(request);

        var existingCustomer = await _customerRepository.GetByIdAsync(request.CustomerId);
        if (existingCustomer == null)
        {
            throw new NotFoundException("Cliente não encontrado");
        }

        var existingPlan = await _planRepository.GetByIdAsync(request.PlanId);
        if (existingPlan == null)
        {
            throw new NotFoundException("Plano não encontrado");
        }

        var alreadyContracted = await _contractRepository.ExistsOpenForCustomerAndPlanAsync(request.CustomerId, request.PlanId);
        if (alreadyContracted)
        {
            throw new ConflictException("Este cliente já possui um contrato aberto para este plano.");
        }

        var contract = new Contract(request.CustomerId, request.PlanId, request.StartDate);
        await _contractRepository.AddAsync(contract);
        await _unitOfWork.SaveChangesAsync();

        return new ContractResponse(contract.Id, contract.CustomerId, contract.PlanId, contract.StartDate, contract.EndDate, contract.Status.ToString());
    }

    private static void Validate(CreateContractRequest request)
    {
        var validator = new CreateContractRequestValidator();

        var result = validator.Validate(request);

        if (result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
