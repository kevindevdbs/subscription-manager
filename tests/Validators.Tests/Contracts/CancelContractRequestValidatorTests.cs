using Shouldly;
using SubscriptionManager.Application.DTOs.Contracts;
using SubscriptionManager.Application.Validators;

namespace Validators.Tests.Contracts;

public class CancelContractRequestValidatorTests
{
    [Fact]
    public void Success()
    {
        var request = new CancelContractRequest(new DateTime(2026, 12, 31));

        var result = new CancelContractRequestValidator().Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEndDateIsDefault()
    {
        var request = new CancelContractRequest(default);

        var result = new CancelContractRequestValidator().Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "A data de encerramento é obrigatória.");
    }
}
