using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Repositories;
using SubscriptionManager.Infrastructure.Data;

namespace SubscriptionManager.Infrastructure.Repositories;

public class ContractRepository : IContractRepository
{

    private readonly AppDbContext _context;
    public ContractRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Contract contract)
    {
        await _context.Contracts.AddAsync(contract);
    }

    public async Task<IEnumerable<Contract>> GetActiveContractsAsync()
    {
        return await _context.Contracts.Where(contract => contract.Status == Domain.Enums.ContractStatus.Active).ToListAsync();
    }

    public async Task<Contract?> GetByIdAsync(Guid id)
    {
        return await _context.Contracts.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Contract>> GetAllAsync()
    {
        return await _context.Contracts
            .AsNoTracking()
            .OrderByDescending(c => c.StartDate)
            .ToListAsync();
    }
}
