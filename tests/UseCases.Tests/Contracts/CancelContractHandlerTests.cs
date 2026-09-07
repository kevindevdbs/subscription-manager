using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using Shouldly;
using SubscriptionManager.Application.DTOs.Contracts;
using SubscriptionManager.Application.UseCases.Contracts;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Exceptions;
using System.Net;

namespace UseCases.Tests.Contracts;

public class CancelContractHandlerTests
{
    [Fact]
    public async Task Success()
    {
        var contract = ContractBuilder.Build();

        var endDate = contract.StartDate.AddMonths(3);

        var result = await CreateHandler(contract).Handle(contract.Id, new CancelContractRequest(endDate));

        result.Status.ShouldBe("Cancelled");
        result.EndDate.ShouldBe(endDate);
    }

    [Fact]
    public async Task Handle_ShouldCancel_WhenContractIsSuspended()
    {
        var contract = ContractBuilder.Build();
        contract.Suspend();

        var request = new CancelContractRequest(contract.StartDate.AddMonths(1));

        var result = await CreateHandler(contract).Handle(contract.Id, request);

        result.Status.ShouldBe("Cancelled");
    }

    [Fact]
    public async Task Handle_ShouldThrowConflict_WhenContractIsAlreadyCancelled()
    {
        var contract = ContractBuilder.Build();
        contract.Cancel(contract.StartDate.AddMonths(1));

        var request = new CancelContractRequest(contract.StartDate.AddMonths(2));

        var exception = await Should.ThrowAsync<ConflictException>(() => CreateHandler(contract).Handle(contract.Id, request));

        exception.GetStatusCode().ShouldBe(HttpStatusCode.Conflict);
        exception.GetErrorMessages().ShouldContain("O contrato já está cancelado.");
    }

    [Fact]
    public async Task Handle_ShouldRejectAnEndDateBeforeTheStartDate()
    {
        var contract = ContractBuilder.Build();

        var request = new CancelContractRequest(contract.StartDate.AddDays(-1));

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(() => CreateHandler(contract).Handle(contract.Id, request));

        exception.GetErrorMessages().ShouldContain("A data de encerramento não pode ser anterior à data de início.");
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenContractDoesNotExist()
    {
        var request = new CancelContractRequest(new DateTime(2026, 10, 1));

        var exception = await Should.ThrowAsync<NotFoundException>(() => CreateHandler(null).Handle(Guid.NewGuid(), request));

        exception.GetErrorMessages().ShouldContain("Contrato não encontrado.");
    }

    [Fact]
    public async Task Handle_ShouldValidateBeforeTouchingTheRepository()
    {
        var exception = await Should.ThrowAsync<ErrorOnValidationException>(
            () => CreateHandler(null).Handle(Guid.NewGuid(), new CancelContractRequest(default)));

        exception.GetErrorMessages().ShouldContain("A data de encerramento é obrigatória.");
    }

    private static CancelContractHandler CreateHandler(Contract? contract)
    {
        var contractBuilder = new IContractRepositoryBuilder();
        if (contract is not null)
        {
            contractBuilder.GetById(contract);
        }

        return new CancelContractHandler(contractBuilder.Build(), IUnitOfWorkBuilder.Build());
    }
}
