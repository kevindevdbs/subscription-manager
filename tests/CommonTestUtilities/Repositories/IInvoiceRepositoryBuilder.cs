using Moq;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Enums;
using SubscriptionManager.Domain.Repositories;

namespace CommonTestUtilities.Repositories;

public class IInvoiceRepositoryBuilder
{
    private readonly Mock<IInvoiceRepository> _mock = new();

    public IInvoiceRepositoryBuilder ExistsForContractAndMonth(Guid contractId, DateTime referenceMonth)
    {
        _mock.Setup(repository => repository.ExistsForContractAndMonthAsync(contractId, referenceMonth)).ReturnsAsync(true);

        return this;
    }

    public IInvoiceRepositoryBuilder GetById(Invoice invoice)
    {
        _mock.Setup(repository => repository.GetByIdAsync(invoice.Id)).ReturnsAsync(invoice);

        return this;
    }

    public IInvoiceRepositoryBuilder GetByContractId(Guid contractId, params Invoice[] invoices)
    {
        _mock.Setup(repository => repository.GetByContractIdAsync(contractId)).ReturnsAsync(invoices);

        return this;
    }

    public IInvoiceRepositoryBuilder GetFiltered(params Invoice[] invoices)
    {
        _mock
            .Setup(repository => repository.GetFilteredAsync(It.IsAny<InvoiceStatus?>(), It.IsAny<DateTime?>()))
            .ReturnsAsync(invoices);

        return this;
    }

    public IInvoiceRepositoryBuilder GetPendingDueBefore(params Invoice[] invoices)
    {
        _mock
            .Setup(repository => repository.GetPendingDueBeforeAsync(It.IsAny<DateTime>()))
            .ReturnsAsync(invoices);

        return this;
    }

    public void VerifyAddedInvoiceDueOn(DateTime dueDate)
    {
        _mock.Verify(repository => repository.AddAsync(It.Is<Invoice>(invoice => invoice.DueDate == dueDate)), Times.Once);
    }

    public void VerifyPendingDueBefore(DateTime referenceDate)
    {
        _mock.Verify(repository => repository.GetPendingDueBeforeAsync(referenceDate), Times.Once);
    }

    public void VerifyAddedInvoices(int times)
    {
        _mock.Verify(repository => repository.AddAsync(It.IsAny<Invoice>()), Times.Exactly(times));
    }

    public IInvoiceRepository Build() => _mock.Object;
}
