using CommonTestUtilities.Requests;
using Shouldly;
using SubscriptionManager.Application.Validators;

namespace Validators.Tests.Plans;

public class CreatePlanRequestValidatorTests
{
    [Fact]
    public void Success()
    {
        var request = CreatePlanRequestBuilder.Build();

        var result = new CreatePlanRequestValidator().Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldHaveError_WhenNameIsEmpty(string name)
    {
        var request = CreatePlanRequestBuilder.Build() with { Name = name };

        var result = new CreatePlanRequestValidator().Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "O nome do plano é obrigatório.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-99.99)]
    public void Validate_ShouldHaveError_WhenMonthlyPriceIsNotPositive(decimal monthlyPrice)
    {
        var request = CreatePlanRequestBuilder.Build() with { MonthlyPrice = monthlyPrice };

        var result = new CreatePlanRequestValidator().Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "O preço mensal deve ser maior que zero.");
    }
}
