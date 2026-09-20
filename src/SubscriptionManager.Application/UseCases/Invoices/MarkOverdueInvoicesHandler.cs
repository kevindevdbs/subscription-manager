using SubscriptionManager.Domain.Repositories;

namespace SubscriptionManager.Application.UseCases.Invoices;

/// <summary>
/// Varre as faturas pendentes já vencidas e marca todas como vencidas de uma vez.
/// </summary>
public class MarkOverdueInvoicesHandler
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public MarkOverdueInvoicesHandler(IInvoiceRepository invoiceRepository, IUnitOfWork unitOfWork, TimeProvider timeProvider)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<int> Handle()
    {
        // A data não vem de quem chama: vencimento é consequência do calendário.
        // Conta a partir do início de hoje porque a fatura vence no fim do dia do
        // vencimento, não à meia-noite dele.
        var today = _timeProvider.GetUtcNow().UtcDateTime.Date;

        var invoices = await _invoiceRepository.GetPendingDueBeforeAsync(today);

        var count = 0;

        foreach (var invoice in invoices)
        {
            // A consulta já filtrou por Pending e vencimento passado, então a
            // entidade não recusa a transição nem deixa a fatura como estava.
            invoice.MarkAsOverdue(today);
            count++;
        }

        await _unitOfWork.SaveChangesAsync();

        return count;
    }
}
