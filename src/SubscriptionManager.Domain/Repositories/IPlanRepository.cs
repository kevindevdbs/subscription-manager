using SubscriptionManager.Domain.Entities;

namespace SubscriptionManager.Domain.Repositories;

public interface IPlanRepository
{
    Task<Plan?> GetByIdAsync(Guid id);

    Task<bool> ExistsActiveWithNameAsync(string name);

    Task<IEnumerable<Plan>> GetAllAsync();

    Task AddAsync(Plan plan);
}
