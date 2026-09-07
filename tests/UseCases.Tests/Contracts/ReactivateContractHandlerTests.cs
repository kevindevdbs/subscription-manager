using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using Shouldly;
using SubscriptionManager.Application.UseCases.Contracts;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Exceptions;
using System.Net;

namespace UseCases.Tests.Contracts;

public class ReactivateContractHandlerTests
{
    [Fact]
    public async Task Success()
    {
        var contract = ContractBuilder.Build();
        contract.Suspend();

        var result = await CreateHandler(contract).Handle(contract.Id);

        result.Status.ShouldBe("Active");
    }

    [Fact]
    public async Task Handle_ShouldThrowConflict_WhenContractIsAlreadyActive()
    {
        var contract = ContractBuilder.Build();

        var exception = await Should.ThrowAsync<ConflictException>(() => CreateHandler(contract).Handle(contract.Id));

        exception.GetStatusCode().ShouldBe(HttpStatusCode.Conflict);
        exception.GetErrorMessages().ShouldContain("O contrato não está suspenso e não pode ser ativado.");
    }

    [Fact]
    public async Task Handle_ShouldThrowConflict_WhenContractIsCancelled()
    {
        var contract = ContractBuilder.Build();
        contract.Cancel(contract.StartDate.AddMonths(1));

        var exception = await Should.ThrowAsync<ConflictException>(() => CreateHandler(contract).Handle(contract.Id));

        exception.GetErrorMessages().ShouldContain("O contrato não está suspenso e não pode ser ativado.");
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenContractDoesNotExist()
    {
        var exception = await Should.ThrowAsync<NotFoundException>(() => CreateHandler(null).Handle(Guid.NewGuid()));

        exception.GetErrorMessages().ShouldContain("Contrato não encontrado.");
    }

    private static ReactivateContractHandler CreateHandler(Contract? contract)
    {
        var contractBuilder = new IContractRepositoryBuilder();
        if (contract is not null)
        {
            contractBuilder.GetById(contract);
        }

        return new ReactivateContractHandler(contractBuilder.Build(), IUnitOfWorkBuilder.Build());
    }
}
