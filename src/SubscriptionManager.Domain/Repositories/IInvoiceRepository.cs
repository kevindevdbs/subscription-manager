using SubscriptionManager.Domain.Entities;

namespace SubscriptionManager.Domain.Repositories;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id);

    Task<bool> ExistsForContractAndMonthAsync(Guid contractId, DateTime referenceMonth);

    Task AddAsync(Invoice invoice);
}
