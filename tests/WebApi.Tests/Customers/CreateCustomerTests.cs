using CommonTestUtilities.Requests;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Net;

namespace WebApi.Tests.Customers;

public class CreateCustomerTests : BaseIntegrationTest
{
    private const string RequestUri = "/api/customers";

    public CreateCustomerTests(SubscriptionManagerApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Success()
    {
        var request = CreateCustomerRequestBuilder.Build();

        var response = await Post(RequestUri, request);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        using var json = await ReadJson(response);

        json.RootElement.GetProperty("name").GetString().ShouldBe(request.Name);
        json.RootElement.GetProperty("email").GetString().ShouldBe(request.Email);

        var persisted = await DbContext.Customers.AnyAsync(customer => customer.Document == request.Document);

        persisted.ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenNameIsEmpty()
    {
        var request = CreateCustomerRequestBuilder.Build() with { Name = string.Empty };

        var response = await Post(RequestUri, request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errors = await ReadErrors(response);

        errors.ShouldContain("O nome é obrigatório.");

        var persisted = await DbContext.Customers.AnyAsync(customer => customer.Document == request.Document);

        persisted.ShouldBeFalse();
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WithEveryError_WhenSeveralFieldsAreInvalid()
    {
        var request = CreateCustomerRequestBuilder.Build() with { Name = string.Empty, Email = "invalido" };

        var response = await Post(RequestUri, request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errors = await ReadErrors(response);

        errors.Count.ShouldBe(2);
    }

    [Fact]
    public async Task ShouldReturnConflict_WhenDocumentIsAlreadyTaken()
    {
        var request = CreateCustomerRequestBuilder.Build();

        var firstResponse = await Post(RequestUri, request);
        firstResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

        var duplicated = CreateCustomerRequestBuilder.Build() with { Document = request.Document };

        var response = await Post(RequestUri, duplicated);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);

        var errors = await ReadErrors(response);

        errors.ShouldContain("Já existe um registro com esses dados.");
    }
}
