using Shouldly;
using SubscriptionManager.Application.DTOs.Invoices;
using SubscriptionManager.Application.Validators;

namespace Validators.Tests.Invoices;

public class MarkInvoiceAsOverdueRequestValidatorTests
{
    [Fact]
    public void Success()
    {
        var request = new MarkInvoiceAsOverdueRequest(new DateTime(2026, 10, 1));

        var result = new MarkInvoiceAsOverdueRequestValidator().Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenReferenceDateIsDefault()
    {
        var request = new MarkInvoiceAsOverdueRequest(default);

        var result = new MarkInvoiceAsOverdueRequestValidator().Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "A data de referência é obrigatória.");
    }
}
