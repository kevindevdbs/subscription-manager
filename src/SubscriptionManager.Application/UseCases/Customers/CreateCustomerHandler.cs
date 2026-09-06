using SubscriptionManager.Application.DTOs.Customers;
using SubscriptionManager.Application.Validators;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Exceptions;
using SubscriptionManager.Domain.Repositories;

namespace SubscriptionManager.Application.UseCases.Customers;

public class CreateCustomerHandler
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork; 
    }


    public async Task<CustomerResponse> Handle(CreateCustomerRequest request)
    {
        Validate(request);

        var customer = new Customer(request.Name, request.Email, request.Document);

        await _customerRepository.AddAsync(customer);
        await _unitOfWork.SaveChangesAsync();

        return new CustomerResponse(customer.Id, customer.Name, customer.Email, customer.Document , customer.CreatedAt);

    }

    private static void Validate(CreateCustomerRequest request)
    {
        var validator = new CreateCustomerRequestValidator();

        var result = validator.Validate(request);

        if (result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
