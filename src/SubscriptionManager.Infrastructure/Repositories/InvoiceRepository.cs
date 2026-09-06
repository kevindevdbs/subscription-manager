using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Domain.Entities;
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
}
