using SubscriptionManager.Application.DTOs.Invoices;
using SubscriptionManager.Application.Mappers;
using SubscriptionManager.Application.Validators;
using SubscriptionManager.Domain.Exceptions;
using SubscriptionManager.Domain.Repositories;

namespace SubscriptionManager.Application.UseCases.Invoices;

public class MarkInvoiceAsOverdueHandler
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkInvoiceAsOverdueHandler(IInvoiceRepository invoiceRepository, IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<InvoiceResponse> Handle(Guid id, MarkInvoiceAsOverdueRequest? request)
    {
        request ??= new MarkInvoiceAsOverdueRequest();

        Validate(request);

        var invoice = await _invoiceRepository.GetByIdAsync(id);
        if (invoice is null)
        {
            throw new NotFoundException("Fatura não encontrada.");
        }

        try
        {
            // A entidade só vira Overdue quando a data de referência passou do
            // vencimento; antes disso a fatura continua Pending e a resposta mostra isso.
            invoice.MarkAsOverdue(request.ReferenceDate ?? DateTime.UtcNow);
        }
        catch (InvalidOperationException exception)
        {
            throw new ConflictException(exception.Message);
        }

        await _unitOfWork.SaveChangesAsync();

        return invoice.ToResponse();
    }

    private static void Validate(MarkInvoiceAsOverdueRequest request)
    {
        var validator = new MarkInvoiceAsOverdueRequestValidator();

        var result = validator.Validate(request);

        if (result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
