using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using Shouldly;
using SubscriptionManager.Application.UseCases.Invoices;
using SubscriptionManager.Domain.Exceptions;
using System.Net;

namespace UseCases.Tests.Invoices;

public class GetInvoicesByContractHandlerTests
{
    [Fact]
    public async Task Success()
    {
        var contract = ContractBuilder.Build();
        var invoice = InvoiceBuilder.Build(contract.Id, amount: 90);

        var handler = new GetInvoicesByContractHandler(
            new IContractRepositoryBuilder().GetById(contract).Build(),
            new IInvoiceRepositoryBuilder().GetByContractId(contract.Id, invoice).Build());

        var result = await handler.Handle(contract.Id);

        result.Count.ShouldBe(1);
        result[0].ContractId.ShouldBe(contract.Id);
        result[0].Amount.ShouldBe(90);
        result[0].Status.ShouldBe("Pending");
    }

    [Fact]
    public async Task Success_ShouldReturnEmptyList_WhenContractHasNoInvoices()
    {
        var contract = ContractBuilder.Build();

        var handler = new GetInvoicesByContractHandler(
            new IContractRepositoryBuilder().GetById(contract).Build(),
            new IInvoiceRepositoryBuilder().GetByContractId(contract.Id).Build());

        var result = await handler.Handle(contract.Id);

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenContractDoesNotExist()
    {
        var handler = new GetInvoicesByContractHandler(
            new IContractRepositoryBuilder().Build(),
            new IInvoiceRepositoryBuilder().Build());

        var exception = await Should.ThrowAsync<NotFoundException>(() => handler.Handle(Guid.NewGuid()));

        exception.GetStatusCode().ShouldBe(HttpStatusCode.NotFound);
        exception.GetErrorMessages().ShouldContain("Contrato não encontrado.");
    }
}
