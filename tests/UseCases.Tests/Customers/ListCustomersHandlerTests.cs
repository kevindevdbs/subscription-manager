using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using Shouldly;
using SubscriptionManager.Application.UseCases.Customers;

namespace UseCases.Tests.Customers;

public class ListCustomersHandlerTests
{
    [Fact]
    public async Task Success()
    {
        var first = CustomerBuilder.Build();
        var second = CustomerBuilder.Build();

        var handler = new ListCustomersHandler(new ICustomerRepositoryBuilder().GetAll(first, second).Build());

        var result = await handler.Handle();

        result.Count.ShouldBe(2);
        result.Select(customer => customer.Id).ShouldBe([first.Id, second.Id]);
        result[0].Name.ShouldBe(first.Name);
    }

    [Fact]
    public async Task Success_ShouldReturnEmptyList_WhenThereIsNoCustomer()
    {
        var handler = new ListCustomersHandler(new ICustomerRepositoryBuilder().GetAll().Build());

        var result = await handler.Handle();

        result.ShouldBeEmpty();
    }
}
