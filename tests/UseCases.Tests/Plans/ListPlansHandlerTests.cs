using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using Shouldly;
using SubscriptionManager.Application.UseCases.Plans;

namespace UseCases.Tests.Plans;

public class ListPlansHandlerTests
{
    [Fact]
    public async Task Success()
    {
        var active = PlanBuilder.Build(monthlyPrice: 90);
        var deactivated = PlanBuilder.Build(monthlyPrice: 150);
        deactivated.Deactivate();

        var handler = new ListPlansHandler(new IPlanRepositoryBuilder().GetAll(active, deactivated).Build());

        var result = await handler.Handle();

        result.Count.ShouldBe(2);
        result[0].MonthlyPrice.ShouldBe(90);
        result[0].IsActive.ShouldBeTrue();
        result[1].IsActive.ShouldBeFalse();
    }

    [Fact]
    public async Task Success_ShouldReturnEmptyList_WhenThereIsNoPlan()
    {
        var handler = new ListPlansHandler(new IPlanRepositoryBuilder().GetAll().Build());

        var result = await handler.Handle();

        result.ShouldBeEmpty();
    }
}
