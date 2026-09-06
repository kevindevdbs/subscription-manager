using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Enums;

namespace SubscriptionManager.Domain.Repositories;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id);

    Task<bool> ExistsForContractAndMonthAsync(Guid contractId, DateTime referenceMonth);

    Task<IEnumerable<Invoice>> GetByContractIdAsync(Guid contractId);

    Task<IEnumerable<Invoice>> GetFilteredAsync(InvoiceStatus? status, DateTime? referenceMonth);

    Task AddAsync(Invoice invoice);
}
