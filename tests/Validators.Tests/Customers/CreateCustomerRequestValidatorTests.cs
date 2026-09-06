using CommonTestUtilities.Requests;
using Shouldly;
using SubscriptionManager.Application.Validators;

namespace Validators.Tests.Customers;

public class CreateCustomerRequestValidatorTests
{
    [Fact]
    public void Success()
    {
        var request = CreateCustomerRequestBuilder.Build();

        var result = new CreateCustomerRequestValidator().Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldHaveError_WhenNameIsEmpty(string name)
    {
        var request = CreateCustomerRequestBuilder.Build() with { Name = name };

        var result = new CreateCustomerRequestValidator().Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "O nome é obrigatório.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNameExceedsMaxLength()
    {
        var request = CreateCustomerRequestBuilder.Build() with { Name = new string('a', 101) };

        var result = new CreateCustomerRequestValidator().Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "O nome deve ter no máximo 100 caracteres.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEmailIsInvalid()
    {
        var request = CreateCustomerRequestBuilder.Build() with { Email = "kevin.email.com" };

        var result = new CreateCustomerRequestValidator().Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "O e-mail informado não é válido.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldHaveError_WhenDocumentIsEmpty(string document)
    {
        var request = CreateCustomerRequestBuilder.Build() with { Document = document };

        var result = new CreateCustomerRequestValidator().Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "O documento é obrigatório.");
    }

    [Fact]
    public void Validate_ShouldReportEveryError_WhenAllFieldsAreInvalid()
    {
        var request = new SubscriptionManager.Application.DTOs.Customers.CreateCustomerRequest(string.Empty, "invalido", string.Empty);

        var result = new CreateCustomerRequestValidator().Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.Select(error => error.PropertyName).Distinct().Count().ShouldBe(3);
    }
}
