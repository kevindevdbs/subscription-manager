using SubscriptionManager.Application.DTOs.Customers;
using SubscriptionManager.Domain.Exceptions;
using SubscriptionManager.Domain.Repositories;

namespace SubscriptionManager.Application.UseCases.Customers;

public class GetCustomerByIdHandler
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByIdHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerResponse> Handle(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer is null)
        {
            throw new NotFoundException("Cliente não encontrado.");
        }

        return new CustomerResponse(customer.Id, customer.Name, customer.Email, customer.Document, customer.CreatedAt);
    }
}
