using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Exceptions;
using SubscriptionManager.Domain.Repositories;

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

    public async Task<int> Handle(DateTime referenceMonth)
    {

        var normalizedMonth = new DateTime(referenceMonth.Year, referenceMonth.Month, 1);

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
                var invoice = new Invoice(contract.Id, plan.MonthlyPrice, normalizedMonth.AddDays(9), normalizedMonth);
                await _invoiceRepository.AddAsync(invoice);
                invoicesCount++;
            }
        }

        await _unitOfWork.SaveChangesAsync();

        return invoicesCount;
    }
}
