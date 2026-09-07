using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.ValueObjects;
using SubscriptionManager.Infrastructure.Data;

namespace SubscriptionManager.Api.Setup;

/// <summary>
/// Aplica migrations e popula dados de demonstração na subida da API.
/// Desligado por padrão, ligado só pelo docker-compose.
/// </summary>
public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        if (!app.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
        {
            return;
        }

        await using var scope = app.Services.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.MigrateAsync();

        if (app.Configuration.GetValue<bool>("Database:SeedOnStartup"))
        {
            await SeedAsync(context);
        }
    }

    private static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Plans.AnyAsync())
        {
            return;
        }

        var basico = new Plan("Básico", new Money(49.90m));
        var academia = new Plan("Academia", new Money(89.90m));
        var premium = new Plan("Premium", new Money(149.90m));
        var legado = new Plan("Legado 2024", new Money(29.90m));

        // Descontinuado: contratos existentes seguem faturando, adesão nova dá 409.
        legado.Deactivate();

        context.Plans.AddRange(basico, academia, premium, legado);

        var kevin = new Customer("Kevin Dias", "kevin@exemplo.com", "12345678901");
        var marina = new Customer("Marina Alves", "marina@exemplo.com", "98765432100");
        var rafael = new Customer("Rafael Souza", "rafael@exemplo.com", "45678912300");

        context.Customers.AddRange(kevin, marina, rafael);

        var startDate = new DateTime(DateTime.UtcNow.Year, 1, 1);

        var kevinAcademia = new Contract(kevin.Id, academia.Id, startDate);
        var kevinBasico = new Contract(kevin.Id, basico.Id, startDate);
        var marinaPremium = new Contract(marina.Id, premium.Id, startDate);
        var rafaelAcademia = new Contract(rafael.Id, academia.Id, startDate);
        rafaelAcademia.Suspend();

        context.Contracts.AddRange(kevinAcademia, kevinBasico, marinaPremium, rafaelAcademia);

        // Estados variados para os filtros de GET /api/invoices terem o que devolver.
        var referenceMonth = new DateTime(DateTime.UtcNow.Year, 1, 1);

        AddInvoice(context, kevinAcademia, academia, referenceMonth, InvoiceOutcome.Paid);
        AddInvoice(context, kevinBasico, basico, referenceMonth, InvoiceOutcome.Paid);
        AddInvoice(context, marinaPremium, premium, referenceMonth, InvoiceOutcome.Paid);

        AddInvoice(context, kevinAcademia, academia, referenceMonth.AddMonths(1), InvoiceOutcome.Overdue);
        AddInvoice(context, kevinBasico, basico, referenceMonth.AddMonths(1), InvoiceOutcome.Paid);
        AddInvoice(context, marinaPremium, premium, referenceMonth.AddMonths(1), InvoiceOutcome.Cancelled);

        AddInvoice(context, kevinAcademia, academia, referenceMonth.AddMonths(2), InvoiceOutcome.Pending);
        AddInvoice(context, kevinBasico, basico, referenceMonth.AddMonths(2), InvoiceOutcome.Pending);
        AddInvoice(context, marinaPremium, premium, referenceMonth.AddMonths(2), InvoiceOutcome.Pending);

        await context.SaveChangesAsync();
    }

    private static void AddInvoice(
        AppDbContext context,
        Contract contract,
        Plan plan,
        DateTime referenceMonth,
        InvoiceOutcome outcome)
    {
        // Money novo: a instância do plano é owned entity rastreada, não pode ter dois donos.
        var invoice = new Invoice(
            contract.Id,
            new Money(plan.MonthlyPrice.Amount),
            referenceMonth.AddDays(9),
            referenceMonth);

        switch (outcome)
        {
            case InvoiceOutcome.Paid:
                invoice.Pay(referenceMonth.AddDays(5));
                break;
            case InvoiceOutcome.Overdue:
                invoice.MarkAsOverdue(referenceMonth.AddDays(20));
                break;
            case InvoiceOutcome.Cancelled:
                invoice.Cancel();
                break;
        }

        context.Invoices.Add(invoice);
    }

    private enum InvoiceOutcome
    {
        Pending,
        Paid,
        Overdue,
        Cancelled
    }
}
