using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using Shouldly;
using SubscriptionManager.Application.UseCases.Contracts;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Exceptions;
using System.Net;

namespace UseCases.Tests.Contracts;

public class SuspendContractHandlerTests
{
    [Fact]
    public async Task Success()
    {
        var contract = ContractBuilder.Build();

        var result = await CreateHandler(contract).Handle(contract.Id);

        result.Id.ShouldBe(contract.Id);
        result.Status.ShouldBe("Suspended");
    }

    [Fact]
    public async Task Handle_ShouldThrowConflict_WhenContractIsAlreadySuspended()
    {
        var contract = ContractBuilder.Build();
        contract.Suspend();

        var exception = await Should.ThrowAsync<ConflictException>(() => CreateHandler(contract).Handle(contract.Id));

        exception.GetStatusCode().ShouldBe(HttpStatusCode.Conflict);
        exception.GetErrorMessages().ShouldContain("O contrato não está ativo e não pode ser suspenso.");
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenContractDoesNotExist()
    {
        var exception = await Should.ThrowAsync<NotFoundException>(() => CreateHandler(null).Handle(Guid.NewGuid()));

        exception.GetErrorMessages().ShouldContain("Contrato não encontrado.");
    }

    private static SuspendContractHandler CreateHandler(Contract? contract)
    {
        var contractBuilder = new IContractRepositoryBuilder();
        if (contract is not null)
        {
            contractBuilder.GetById(contract);
        }

        return new SuspendContractHandler(contractBuilder.Build(), IUnitOfWorkBuilder.Build());
    }
}
