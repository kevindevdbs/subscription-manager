using CommonTestUtilities.Requests;
using Shouldly;
using SubscriptionManager.Application.Validators;

namespace Validators.Tests.Contracts;

public class CreateContractRequestValidatorTests
{
    [Fact]
    public void Success()
    {
        var request = CreateContractRequestBuilder.Build();

        var result = new CreateContractRequestValidator().Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenCustomerIdIsEmpty()
    {
        var request = CreateContractRequestBuilder.Build() with { CustomerId = Guid.Empty };

        var result = new CreateContractRequestValidator().Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "O identificador do cliente é obrigatório.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPlanIdIsEmpty()
    {
        var request = CreateContractRequestBuilder.Build() with { PlanId = Guid.Empty };

        var result = new CreateContractRequestValidator().Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "O identificador do plano é obrigatório.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenStartDateIsDefault()
    {
        var request = CreateContractRequestBuilder.Build() with { StartDate = default };

        var result = new CreateContractRequestValidator().Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "A data de início é obrigatória.");
    }
}
