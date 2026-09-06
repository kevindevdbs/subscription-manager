using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using Shouldly;
using SubscriptionManager.Application.UseCases.Plans;
using SubscriptionManager.Domain.Exceptions;
using System.Net;

namespace UseCases.Tests.Plans;

public class GetPlanByIdHandlerTests
{
    [Fact]
    public async Task Success()
    {
        var plan = PlanBuilder.Build(monthlyPrice: 90);

        var handler = new GetPlanByIdHandler(new IPlanRepositoryBuilder().GetById(plan).Build());

        var result = await handler.Handle(plan.Id);

        result.Id.ShouldBe(plan.Id);
        result.MonthlyPrice.ShouldBe(90);
        result.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenPlanDoesNotExist()
    {
        var handler = new GetPlanByIdHandler(new IPlanRepositoryBuilder().Build());

        var exception = await Should.ThrowAsync<NotFoundException>(() => handler.Handle(Guid.NewGuid()));

        exception.GetStatusCode().ShouldBe(HttpStatusCode.NotFound);
        exception.GetErrorMessages().ShouldContain("Plano não encontrado.");
    }
}
