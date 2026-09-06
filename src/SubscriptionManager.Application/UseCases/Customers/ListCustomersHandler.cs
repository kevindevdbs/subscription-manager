using SubscriptionManager.Application.DTOs.Customers;
using SubscriptionManager.Domain.Repositories;

namespace SubscriptionManager.Application.UseCases.Customers;

public class ListCustomersHandler
{
    private readonly ICustomerRepository _customerRepository;

    public ListCustomersHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IReadOnlyList<CustomerResponse>> Handle()
    {
        var customers = await _customerRepository.GetAllAsync();

        return customers
            .Select(customer => new CustomerResponse(
                customer.Id,
                customer.Name,
                customer.Email,
                customer.Document,
                customer.CreatedAt))
            .ToList();
    }
}
