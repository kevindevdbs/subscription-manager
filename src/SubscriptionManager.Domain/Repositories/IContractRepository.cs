using SubscriptionManager.Domain.Entities;

namespace SubscriptionManager.Domain.Repositories;

public interface IContractRepository
{
    Task<Contract?> GetByIdAsync(Guid id);

    Task<IEnumerable<Contract>> GetActiveContractsAsync();

    Task<IEnumerable<Contract>> GetAllAsync();

    Task<bool> ExistsOpenForCustomerAndPlanAsync(Guid customerId, Guid planId);

    Task AddAsync(Contract contract);
}
