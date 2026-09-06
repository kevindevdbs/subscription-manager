using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using Shouldly;
using SubscriptionManager.Application.UseCases.Contracts;
using SubscriptionManager.Domain.Exceptions;
using System.Net;

namespace UseCases.Tests.Contracts;

public class GetContractByIdHandlerTests
{
    [Fact]
    public async Task Success()
    {
        var contract = ContractBuilder.Build();

        var handler = new GetContractByIdHandler(new IContractRepositoryBuilder().GetById(contract).Build());

        var result = await handler.Handle(contract.Id);

        result.Id.ShouldBe(contract.Id);
        result.CustomerId.ShouldBe(contract.CustomerId);
        result.Status.ShouldBe("Active");
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenContractDoesNotExist()
    {
        var handler = new GetContractByIdHandler(new IContractRepositoryBuilder().Build());

        var exception = await Should.ThrowAsync<NotFoundException>(() => handler.Handle(Guid.NewGuid()));

        exception.GetStatusCode().ShouldBe(HttpStatusCode.NotFound);
        exception.GetErrorMessages().ShouldContain("Contrato não encontrado.");
    }
}
