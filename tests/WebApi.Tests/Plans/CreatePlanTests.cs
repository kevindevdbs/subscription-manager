using CommonTestUtilities.Requests;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Net;

namespace WebApi.Tests.Plans;

public class CreatePlanTests : BaseIntegrationTest
{
    private const string RequestUri = "/api/plans";

    public CreatePlanTests(SubscriptionManagerApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Success()
    {
        var request = CreatePlanRequestBuilder.Build();

        var response = await Post(RequestUri, request);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        using var json = await ReadJson(response);

        json.RootElement.GetProperty("name").GetString().ShouldBe(request.Name);
        json.RootElement.GetProperty("isActive").GetBoolean().ShouldBeTrue();

        var persisted = await DbContext.Plans.AnyAsync(plan => plan.Name == request.Name);

        persisted.ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenMonthlyPriceIsZero()
    {
        var request = CreatePlanRequestBuilder.Build() with { MonthlyPrice = 0 };

        var response = await Post(RequestUri, request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errors = await ReadErrors(response);

        errors.ShouldContain("O preço mensal deve ser maior que zero.");
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenAnActivePlanAlreadyHasTheName()
    {
        var request = CreatePlanRequestBuilder.Build();

        var firstResponse = await Post(RequestUri, request);
        firstResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

        var response = await Post(RequestUri, request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errors = await ReadErrors(response);

        errors.ShouldContain("Já existe um plano ativo com esse nome.");

        var count = await DbContext.Plans.CountAsync(plan => plan.Name == request.Name);

        count.ShouldBe(1);
    }
}
