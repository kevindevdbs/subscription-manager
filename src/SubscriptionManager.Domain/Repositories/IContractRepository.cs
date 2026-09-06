using SubscriptionManager.Domain.Entities;

namespace SubscriptionManager.Domain.Repositories;

public interface IContractRepository
{
    Task<Contract?> GetByIdAsync(Guid id);

    Task<IEnumerable<Contract>> GetActiveContractsAsync();

    Task<IEnumerable<Contract>> GetAllAsync();

    Task AddAsync(Contract contract);
}
