using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using Shouldly;
using SubscriptionManager.Application.UseCases.Contracts;

namespace UseCases.Tests.Contracts;

public class ListContractsHandlerTests
{
    [Fact]
    public async Task Success()
    {
        var active = ContractBuilder.Build();
        var cancelled = ContractBuilder.Build();
        cancelled.Cancel(new DateTime(2026, 9, 30));

        var handler = new ListContractsHandler(new IContractRepositoryBuilder().GetAll(active, cancelled).Build());

        var result = await handler.Handle();

        result.Count.ShouldBe(2);
        result[0].Status.ShouldBe("Active");
        result[0].EndDate.ShouldBeNull();
        result[1].Status.ShouldBe("Cancelled");
        result[1].EndDate.ShouldBe(new DateTime(2026, 9, 30));
    }

    [Fact]
    public async Task Success_ShouldReturnEmptyList_WhenThereIsNoContract()
    {
        var handler = new ListContractsHandler(new IContractRepositoryBuilder().GetAll().Build());

        var result = await handler.Handle();

        result.ShouldBeEmpty();
    }
}
