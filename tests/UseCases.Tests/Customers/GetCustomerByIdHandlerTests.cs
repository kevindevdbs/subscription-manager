using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using Shouldly;
using SubscriptionManager.Application.UseCases.Customers;
using SubscriptionManager.Domain.Exceptions;
using System.Net;

namespace UseCases.Tests.Customers;

public class GetCustomerByIdHandlerTests
{
    [Fact]
    public async Task Success()
    {
        var customer = CustomerBuilder.Build();

        var handler = new GetCustomerByIdHandler(new ICustomerRepositoryBuilder().GetById(customer).Build());

        var result = await handler.Handle(customer.Id);

        result.Id.ShouldBe(customer.Id);
        result.Name.ShouldBe(customer.Name);
        result.Email.ShouldBe(customer.Email);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenCustomerDoesNotExist()
    {
        var handler = new GetCustomerByIdHandler(new ICustomerRepositoryBuilder().Build());

        var exception = await Should.ThrowAsync<NotFoundException>(() => handler.Handle(Guid.NewGuid()));

        exception.GetStatusCode().ShouldBe(HttpStatusCode.NotFound);
        exception.GetErrorMessages().ShouldContain("Cliente não encontrado.");
    }
}
