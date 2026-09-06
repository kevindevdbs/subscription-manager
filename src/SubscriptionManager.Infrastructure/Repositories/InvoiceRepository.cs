using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Enums;
using SubscriptionManager.Domain.Repositories;
using SubscriptionManager.Infrastructure.Data;

namespace SubscriptionManager.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{

    private readonly AppDbContext _context;
    public InvoiceRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(Invoice invoice)
    {
        await _context.Invoices.AddAsync(invoice);
    }

    public async Task<bool> ExistsForContractAndMonthAsync(Guid contractId, DateTime referenceMonth)
    {
        return await _context.Invoices.AnyAsync(i => i.ContractId == contractId && i.ReferenceMonth == referenceMonth);
    }

    public async Task<Invoice?> GetByIdAsync(Guid id)
    {
        return await _context.Invoices.FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<Invoice>> GetByContractIdAsync(Guid contractId)
    {
        return await _context.Invoices
            .AsNoTracking()
            .Where(i => i.ContractId == contractId)
            .OrderByDescending(i => i.ReferenceMonth)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetFilteredAsync(InvoiceStatus? status, DateTime? referenceMonth)
    {
        var query = _context.Invoices.AsNoTracking();

        if (status.HasValue)
        {
            query = query.Where(i => i.Status == status.Value);
        }

        if (referenceMonth.HasValue)
        {
            query = query.Where(i => i.ReferenceMonth == referenceMonth.Value);
        }

        return await query
            .OrderBy(i => i.DueDate)
            .ToListAsync();
    }
}
