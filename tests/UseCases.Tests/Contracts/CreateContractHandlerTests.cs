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

    private static CreateContractHandler CreateHandler(Customer? customer, Plan? plan)
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

        return new CreateContractHandler(
            new IContractRepositoryBuilder().Build(),
            IUnitOfWorkBuilder.Build(),
            customerBuilder.Build(),
            planBuilder.Build());
    }
}
