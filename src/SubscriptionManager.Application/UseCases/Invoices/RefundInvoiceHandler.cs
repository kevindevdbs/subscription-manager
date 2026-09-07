using SubscriptionManager.Application.DTOs.Invoices;
using SubscriptionManager.Application.Mappers;
using SubscriptionManager.Domain.Exceptions;
using SubscriptionManager.Domain.Repositories;

namespace SubscriptionManager.Application.UseCases.Invoices;

public class RefundInvoiceHandler
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RefundInvoiceHandler(IInvoiceRepository invoiceRepository, IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<InvoiceResponse> Handle(Guid id)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id);
        if (invoice is null)
        {
            throw new NotFoundException("Fatura não encontrada.");
        }

        try
        {
            invoice.Refund();
        }
        catch (InvalidOperationException exception)
        {
            throw new ConflictException(exception.Message);
        }

        await _unitOfWork.SaveChangesAsync();

        return invoice.ToResponse();
    }
}
