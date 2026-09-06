using SubscriptionManager.Domain.Entities;

namespace SubscriptionManager.Domain.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id);

    Task AddAsync(Customer customer);
}
