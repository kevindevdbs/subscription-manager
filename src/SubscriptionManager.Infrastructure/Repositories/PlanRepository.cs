using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Repositories;
using SubscriptionManager.Infrastructure.Data;

namespace SubscriptionManager.Infrastructure.Repositories;

public class PlanRepository : IPlanRepository
{

    private readonly AppDbContext _context;

    public PlanRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(Plan plan)
    {
        await _context.Plans.AddAsync(plan);
    }

    public async Task<Plan?> GetByIdAsync(Guid id)
    {
        return await _context.Plans.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> ExistsActiveWithNameAsync(string name)
    {
        return await _context.Plans.AnyAsync(p => p.Name == name && p.IsActive);
    }

    public async Task<IEnumerable<Plan>> GetAllAsync()
    {
        return await _context.Plans
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync();
    }
}
