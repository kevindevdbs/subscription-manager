using SubscriptionManager.Domain.Entities;

namespace SubscriptionManager.Domain.Repositories;

public interface IPlanRepository
{
    Task<Plan?> GetByIdAsync(Guid id);

    Task AddAsync(Plan plan);
}
