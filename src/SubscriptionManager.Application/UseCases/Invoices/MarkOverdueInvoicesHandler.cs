using SubscriptionManager.Application.DTOs.Invoices;
using SubscriptionManager.Application.Validators;
using SubscriptionManager.Domain.Exceptions;
using SubscriptionManager.Domain.Repositories;

namespace SubscriptionManager.Application.UseCases.Invoices;

/// <summary>
/// Varre as faturas pendentes já vencidas e marca todas como vencidas de uma vez.
/// </summary>
public class MarkOverdueInvoicesHandler
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkOverdueInvoicesHandler(IInvoiceRepository invoiceRepository, IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(MarkInvoiceAsOverdueRequest? request)
    {
        request ??= new MarkInvoiceAsOverdueRequest();

        Validate(request);

        var referenceDate = request.ReferenceDate ?? DateTime.UtcNow;

        var invoices = await _invoiceRepository.GetPendingDueBeforeAsync(referenceDate);

        var count = 0;

        foreach (var invoice in invoices)
        {
            // A consulta já filtrou por Pending e vencimento passado, então a
            // entidade não recusa a transição nem deixa a fatura como estava.
            invoice.MarkAsOverdue(referenceDate);
            count++;
        }

        await _unitOfWork.SaveChangesAsync();

        return count;
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
