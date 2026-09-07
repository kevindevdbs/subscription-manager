using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using Shouldly;
using SubscriptionManager.Application.UseCases.Plans;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Exceptions;
using System.Net;

namespace UseCases.Tests.Plans;

public class DeactivatePlanHandlerTests
{
    [Fact]
    public async Task Success()
    {
        var plan = PlanBuilder.Build();

        var result = await CreateHandler(plan).Handle(plan.Id);

        result.Id.ShouldBe(plan.Id);
        result.IsActive.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_ShouldThrowConflict_WhenPlanIsAlreadyDeactivated()
    {
        var plan = PlanBuilder.Build();
        plan.Deactivate();

        var exception = await Should.ThrowAsync<ConflictException>(() => CreateHandler(plan).Handle(plan.Id));

        exception.GetStatusCode().ShouldBe(HttpStatusCode.Conflict);
        exception.GetErrorMessages().ShouldContain("O plano já está desativado.");
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenPlanDoesNotExist()
    {
        var exception = await Should.ThrowAsync<NotFoundException>(() => CreateHandler(null).Handle(Guid.NewGuid()));

        exception.GetErrorMessages().ShouldContain("Plano não encontrado.");
    }

    private static DeactivatePlanHandler CreateHandler(Plan? plan)
    {
        var planBuilder = new IPlanRepositoryBuilder();
        if (plan is not null)
        {
            planBuilder.GetById(plan);
        }

        return new DeactivatePlanHandler(planBuilder.Build(), IUnitOfWorkBuilder.Build());
    }
}
