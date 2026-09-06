using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using Shouldly;
using SubscriptionManager.Application.DTOs.Invoices;
using SubscriptionManager.Application.UseCases.Invoices;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Exceptions;

namespace UseCases.Tests.Invoices;

public class GenerateMonthlyInvoicesHandlerTests
{
    private static readonly DateTime ReferenceMonth = new(2026, 9, 1);

    [Fact]
    public async Task Success_ShouldGenerateOneInvoicePerActiveContract()
    {
        var plan = PlanBuilder.Build(monthlyPrice: 90);
        var firstContract = ContractBuilder.Build(planId: plan.Id);
        var secondContract = ContractBuilder.Build(planId: plan.Id);

        var invoiceRepository = new IInvoiceRepositoryBuilder();

        var handler = CreateHandler(invoiceRepository, plan, firstContract, secondContract);

        var generated = await handler.Handle(new GenerateInvoiceRequest(ReferenceMonth));

        generated.ShouldBe(2);
        invoiceRepository.VerifyAddedInvoices(2);
    }

    [Fact]
    public async Task Handle_ShouldBeIdempotent_WhenInvoiceAlreadyExistsForTheMonth()
    {
        var plan = PlanBuilder.Build();
        var contract = ContractBuilder.Build(planId: plan.Id);

        var invoiceRepository = new IInvoiceRepositoryBuilder().ExistsForContractAndMonth(contract.Id, ReferenceMonth);

        var handler = CreateHandler(invoiceRepository, plan, contract);

        var generated = await handler.Handle(new GenerateInvoiceRequest(ReferenceMonth));

        generated.ShouldBe(0);
        invoiceRepository.VerifyAddedInvoices(0);
    }

    [Fact]
    public async Task Handle_ShouldNormalizeReferenceMonthToTheFirstDay()
    {
        var plan = PlanBuilder.Build();
        var contract = ContractBuilder.Build(planId: plan.Id);

        var invoiceRepository = new IInvoiceRepositoryBuilder().ExistsForContractAndMonth(contract.Id, ReferenceMonth);

        var handler = CreateHandler(invoiceRepository, plan, contract);

        var generated = await handler.Handle(new GenerateInvoiceRequest(new DateTime(2026, 9, 23)));

        generated.ShouldBe(0);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenTheContractPlanIsMissing()
    {
        var contract = ContractBuilder.Build();

        var handler = new GenerateMonthlyInvoicesHandler(
            new IContractRepositoryBuilder().GetActiveContracts(contract).Build(),
            new IInvoiceRepositoryBuilder().Build(),
            IUnitOfWorkBuilder.Build(),
            new IPlanRepositoryBuilder().Build());

        var exception = await Should.ThrowAsync<NotFoundException>(() => handler.Handle(new GenerateInvoiceRequest(ReferenceMonth)));

        exception.GetErrorMessages().ShouldContain("Plano vinculado a este contrato não encontrado.");
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenReferenceMonthIsDefault()
    {
        var handler = CreateHandler(new IInvoiceRepositoryBuilder(), PlanBuilder.Build());

        await Should.ThrowAsync<ErrorOnValidationException>(() => handler.Handle(new GenerateInvoiceRequest(default)));
    }

    private static GenerateMonthlyInvoicesHandler CreateHandler(
        IInvoiceRepositoryBuilder invoiceRepository,
        Plan plan,
        params Contract[] contracts)
    {
        return new GenerateMonthlyInvoicesHandler(
            new IContractRepositoryBuilder().GetActiveContracts(contracts).Build(),
            invoiceRepository.Build(),
            IUnitOfWorkBuilder.Build(),
            new IPlanRepositoryBuilder().GetById(plan).Build());
    }
}
