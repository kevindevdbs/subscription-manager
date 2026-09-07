using SubscriptionManager.Application.DTOs.Invoices;
using SubscriptionManager.Application.Mappers;
using SubscriptionManager.Application.Validators;
using SubscriptionManager.Domain.Exceptions;
using SubscriptionManager.Domain.Repositories;

namespace SubscriptionManager.Application.UseCases.Invoices;

public class ListInvoicesHandler
{
    private readonly IInvoiceRepository _invoiceRepository;

    public ListInvoicesHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<IReadOnlyList<InvoiceResponse>> Handle(ListInvoicesRequest request)
    {
        Validate(request);

        request.TryGetStatus(out var status);
        request.TryGetMonth(out var referenceMonth);

        var invoices = await _invoiceRepository.GetFilteredAsync(status, referenceMonth);

        return invoices.ToResponse();
    }

    private static void Validate(ListInvoicesRequest request)
    {
        var validator = new ListInvoicesRequestValidator();

        var result = validator.Validate(request);

        if (result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
