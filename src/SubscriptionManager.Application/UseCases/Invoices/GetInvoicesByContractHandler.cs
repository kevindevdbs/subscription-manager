using SubscriptionManager.Application.DTOs.Invoices;
using SubscriptionManager.Domain.Exceptions;
using SubscriptionManager.Domain.Repositories;

namespace SubscriptionManager.Application.UseCases.Invoices;

public class GetInvoicesByContractHandler
{
    private readonly IContractRepository _contractRepository;
    private readonly IInvoiceRepository _invoiceRepository;

    public GetInvoicesByContractHandler(IContractRepository contractRepository, IInvoiceRepository invoiceRepository)
    {
        _contractRepository = contractRepository;
        _invoiceRepository = invoiceRepository;
    }

    public async Task<IReadOnlyList<InvoiceResponse>> Handle(Guid contractId)
    {
        var contract = await _contractRepository.GetByIdAsync(contractId);
        if (contract is null)
        {
            throw new NotFoundException("Contrato não encontrado.");
        }

        var invoices = await _invoiceRepository.GetByContractIdAsync(contractId);

        return invoices
            .Select(invoice => new InvoiceResponse(
                invoice.Id,
                invoice.ContractId,
                invoice.Amount.Amount,
                invoice.DueDate,
                invoice.ReferenceMonth,
                invoice.PaidAt,
                invoice.Status.ToString()))
            .ToList();
    }
}
