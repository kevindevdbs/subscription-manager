using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using Shouldly;
using SubscriptionManager.Application.UseCases.Customers;
using SubscriptionManager.Domain.Exceptions;
using System.Net;

namespace UseCases.Tests.Customers;

public class CreateCustomerHandlerTests
{
    [Fact]
    public async Task Success()
    {
        var request = CreateCustomerRequestBuilder.Build();

        var result = await CreateHandler().Handle(request);

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe(request.Name);
        result.Email.ShouldBe(request.Email);
        result.Document.ShouldBe(request.Document);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenNameIsEmpty()
    {
        var request = CreateCustomerRequestBuilder.Build() with { Name = string.Empty };

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(() => CreateHandler().Handle(request));

        exception.GetStatusCode().ShouldBe(HttpStatusCode.BadRequest);
        exception.GetErrorMessages().ShouldContain("O nome é obrigatório.");
    }

    [Fact]
    public async Task Handle_ShouldReportEveryError_WhenSeveralFieldsAreInvalid()
    {
        var request = CreateCustomerRequestBuilder.Build() with { Name = string.Empty, Document = string.Empty };

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(() => CreateHandler().Handle(request));

        exception.GetErrorMessages().Count.ShouldBe(2);
    }

    private static CreateCustomerHandler CreateHandler()
    {
        return new CreateCustomerHandler(new ICustomerRepositoryBuilder().Build(), IUnitOfWorkBuilder.Build());
    }
}
