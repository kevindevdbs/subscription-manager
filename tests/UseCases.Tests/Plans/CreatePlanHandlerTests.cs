using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using Shouldly;
using SubscriptionManager.Application.UseCases.Plans;
using SubscriptionManager.Domain.Exceptions;
using System.Net;

namespace UseCases.Tests.Plans;

public class CreatePlanHandlerTests
{
    [Fact]
    public async Task Success()
    {
        var request = CreatePlanRequestBuilder.Build();

        var handler = new CreatePlanHandler(new IPlanRepositoryBuilder().Build(), IUnitOfWorkBuilder.Build());

        var result = await handler.Handle(request);

        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe(request.Name);
        result.MonthlyPrice.ShouldBe(request.MonthlyPrice);
        result.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenMonthlyPriceIsZero()
    {
        var request = CreatePlanRequestBuilder.Build() with { MonthlyPrice = 0 };

        var handler = new CreatePlanHandler(new IPlanRepositoryBuilder().Build(), IUnitOfWorkBuilder.Build());

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(() => handler.Handle(request));

        exception.GetStatusCode().ShouldBe(HttpStatusCode.BadRequest);
        exception.GetErrorMessages().ShouldContain("O preço mensal deve ser maior que zero.");
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenAnActivePlanAlreadyHasTheName()
    {
        var request = CreatePlanRequestBuilder.Build();

        var planRepository = new IPlanRepositoryBuilder().ExistsActiveWithName(request.Name).Build();

        var handler = new CreatePlanHandler(planRepository, IUnitOfWorkBuilder.Build());

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(() => handler.Handle(request));

        exception.GetErrorMessages().ShouldContain("Já existe um plano ativo com esse nome.");
    }

    [Fact]
    public async Task Handle_ShouldReportFormatAndDuplicateErrorsTogether()
    {
        var request = CreatePlanRequestBuilder.Build() with { MonthlyPrice = 0 };

        var planRepository = new IPlanRepositoryBuilder().ExistsActiveWithName(request.Name).Build();

        var handler = new CreatePlanHandler(planRepository, IUnitOfWorkBuilder.Build());

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(() => handler.Handle(request));

        exception.GetErrorMessages().Count.ShouldBe(2);
    }
}
