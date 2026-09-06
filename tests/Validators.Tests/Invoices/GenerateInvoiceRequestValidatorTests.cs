using Shouldly;
using SubscriptionManager.Application.DTOs.Invoices;
using SubscriptionManager.Application.Validators;

namespace Validators.Tests.Invoices;

public class GenerateInvoiceRequestValidatorTests
{
    [Fact]
    public void Success()
    {
        var request = new GenerateInvoiceRequest(new DateTime(2026, 9, 1));

        var result = new GenerateInvoiceRequestValidator().Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenReferenceMonthIsDefault()
    {
        var request = new GenerateInvoiceRequest(default);

        var result = new GenerateInvoiceRequestValidator().Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "O mês de referência é obrigatório.");
    }
}
