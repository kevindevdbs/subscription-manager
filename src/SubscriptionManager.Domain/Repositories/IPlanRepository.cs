using SubscriptionManager.Domain.Entities;

namespace SubscriptionManager.Domain.Repositories;

public interface IPlanRepository
{
    Task<Plan?> GetByIdAsync(Guid id);

    Task<bool> ExistsActiveWithNameAsync(string name);

    Task AddAsync(Plan plan);
}
