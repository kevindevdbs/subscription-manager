using SubscriptionManager.Application.DTOs.Invoices;
using SubscriptionManager.Application.Mappers;
using SubscriptionManager.Application.Validators;
using SubscriptionManager.Domain.Exceptions;
using SubscriptionManager.Domain.Repositories;

namespace SubscriptionManager.Application.UseCases.Invoices;

public class PayInvoiceHandler
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PayInvoiceHandler(IInvoiceRepository invoiceRepository, IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<InvoiceResponse> Handle(Guid id, PayInvoiceRequest? request)
    {
        request ??= new PayInvoiceRequest();

        Validate(request);

        var invoice = await _invoiceRepository.GetByIdAsync(id);
        if (invoice is null)
        {
            throw new NotFoundException("Fatura não encontrada.");
        }

        invoice.Pay(request.PaidAt ?? DateTime.UtcNow);

        await _unitOfWork.SaveChangesAsync();

        return invoice.ToResponse();
    }

    private static void Validate(PayInvoiceRequest request)
    {
        var validator = new PayInvoiceRequestValidator();

        var result = validator.Validate(request);

        if (result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
