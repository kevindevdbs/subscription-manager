using SubscriptionManager.Application.DTOs.Invoices;
using SubscriptionManager.Application.Validators;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Exceptions;
using SubscriptionManager.Domain.Repositories;
using SubscriptionManager.Domain.ValueObjects;

namespace SubscriptionManager.Application.UseCases.Invoices;

public class GenerateMonthlyInvoicesHandler
{
    private readonly IContractRepository _contractRepository;
    private readonly IInvoiceRepository _invoiceRepository;

    private readonly IPlanRepository _planRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GenerateMonthlyInvoicesHandler(IContractRepository contractRepository, IInvoiceRepository invoiceRepository, IUnitOfWork unitOfWork, IPlanRepository planRepository)
    {
        _contractRepository = contractRepository;
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
        _planRepository = planRepository;
    }

    public async Task<int> Handle(GenerateInvoiceRequest request)
    {
        Validate(request);

        var normalizedMonth = new DateTime(request.ReferenceMonth.Year, request.ReferenceMonth.Month, 1);

        var contracts = await _contractRepository.GetActiveContractsAsync();

        int invoicesCount = 0;

        foreach (var contract in contracts)
        {
            var invoiceExists = await _invoiceRepository.ExistsForContractAndMonthAsync(contract.Id, normalizedMonth);

            if (!invoiceExists)
            {
                var plan = await _planRepository.GetByIdAsync(contract.PlanId);
                if (plan is null)
                {
                    throw new NotFoundException("Plano vinculado a este contrato não encontrado.");
                }
                // Money é owned entity do plano e da fatura. Reaproveitar a instância do
                // plano faria dois donos apontarem para o mesmo objeto rastreado e o
                // SaveChanges quebraria assim que dois contratos usassem o mesmo plano.
                var amount = new Money(plan.MonthlyPrice.Amount);
                var invoice = new Invoice(contract.Id, amount, normalizedMonth.AddDays(9), normalizedMonth);
                await _invoiceRepository.AddAsync(invoice);
                invoicesCount++;
            }
        }

        await _unitOfWork.SaveChangesAsync();

        return invoicesCount;
    }

    private static void Validate(GenerateInvoiceRequest request)
    {
        var validator = new GenerateInvoiceRequestValidator();

        var result = validator.Validate(request);

        if (result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
