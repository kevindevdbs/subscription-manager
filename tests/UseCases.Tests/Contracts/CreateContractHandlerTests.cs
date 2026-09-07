using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using Shouldly;
using SubscriptionManager.Application.UseCases.Contracts;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Exceptions;
using System.Net;

namespace UseCases.Tests.Contracts;

public class CreateContractHandlerTests
{
    [Fact]
    public async Task Success()
    {
        var customer = CustomerBuilder.Build();
        var plan = PlanBuilder.Build();

        var request = CreateContractRequestBuilder.Build(customer.Id, plan.Id);

        var result = await CreateHandler(customer, plan).Handle(request);

        result.Id.ShouldNotBe(Guid.Empty);
        result.CustomerId.ShouldBe(customer.Id);
        result.PlanId.ShouldBe(plan.Id);
        result.Status.ShouldBe("Active");
        result.EndDate.ShouldBeNull();
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenCustomerDoesNotExist()
    {
        var plan = PlanBuilder.Build();

        var request = CreateContractRequestBuilder.Build(Guid.NewGuid(), plan.Id);

        var exception = await Should.ThrowAsync<NotFoundException>(() => CreateHandler(null, plan).Handle(request));

        exception.GetStatusCode().ShouldBe(HttpStatusCode.NotFound);
        exception.GetErrorMessages().ShouldContain("Cliente não encontrado");
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenPlanDoesNotExist()
    {
        var customer = CustomerBuilder.Build();

        var request = CreateContractRequestBuilder.Build(customer.Id, Guid.NewGuid());

        var exception = await Should.ThrowAsync<NotFoundException>(() => CreateHandler(customer, null).Handle(request));

        exception.GetErrorMessages().ShouldContain("Plano não encontrado");
    }

    [Fact]
    public async Task Handle_ShouldValidateBeforeTouchingTheRepositories()
    {
        var request = CreateContractRequestBuilder.Build() with { CustomerId = Guid.Empty };

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(() => CreateHandler(null, null).Handle(request));

        exception.GetErrorMessages().ShouldContain("O identificador do cliente é obrigatório.");
    }

    [Fact]
    public async Task Handle_ShouldThrowConflict_WhenPlanIsDeactivated()
    {
        var customer = CustomerBuilder.Build();
        var plan = PlanBuilder.Build();
        plan.Deactivate();

        var request = CreateContractRequestBuilder.Build(customer.Id, plan.Id);

        var exception = await Should.ThrowAsync<ConflictException>(() => CreateHandler(customer, plan).Handle(request));

        exception.GetStatusCode().ShouldBe(HttpStatusCode.Conflict);
        exception.GetErrorMessages().ShouldContain("Este plano está desativado e não aceita novos contratos.");
    }

    [Fact]
    public async Task Handle_ShouldThrowConflict_WhenCustomerAlreadyHasThePlan()
    {
        var customer = CustomerBuilder.Build();
        var plan = PlanBuilder.Build();

        var request = CreateContractRequestBuilder.Build(customer.Id, plan.Id);

        var handler = CreateHandler(customer, plan, alreadyContracted: true);

        var exception = await Should.ThrowAsync<ConflictException>(() => handler.Handle(request));

        exception.GetStatusCode().ShouldBe(HttpStatusCode.Conflict);
        exception.GetErrorMessages().ShouldContain("Este cliente já possui um contrato aberto para este plano.");
    }

    [Fact]
    public async Task Handle_ShouldAllowTheSamePlanForADifferentCustomer()
    {
        var customer = CustomerBuilder.Build();
        var plan = PlanBuilder.Build();

        var request = CreateContractRequestBuilder.Build(customer.Id, plan.Id);

        var contractBuilder = new IContractRepositoryBuilder()
            .ExistsOpenForCustomerAndPlan(Guid.NewGuid(), plan.Id);

        var handler = new CreateContractHandler(
            contractBuilder.Build(),
            IUnitOfWorkBuilder.Build(),
            new ICustomerRepositoryBuilder().GetById(customer).Build(),
            new IPlanRepositoryBuilder().GetById(plan).Build());

        var result = await handler.Handle(request);

        result.Status.ShouldBe("Active");
    }

    private static CreateContractHandler CreateHandler(Customer? customer, Plan? plan, bool alreadyContracted = false)
    {
        var customerBuilder = new ICustomerRepositoryBuilder();
        if (customer is not null)
        {
            customerBuilder.GetById(customer);
        }

        var planBuilder = new IPlanRepositoryBuilder();
        if (plan is not null)
        {
            planBuilder.GetById(plan);
        }

        var contractBuilder = new IContractRepositoryBuilder();
        if (alreadyContracted && customer is not null && plan is not null)
        {
            contractBuilder.ExistsOpenForCustomerAndPlan(customer.Id, plan.Id);
        }

        return new CreateContractHandler(
            contractBuilder.Build(),
            IUnitOfWorkBuilder.Build(),
            customerBuilder.Build(),
            planBuilder.Build());
    }
}
