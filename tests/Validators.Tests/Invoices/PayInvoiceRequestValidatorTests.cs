using Shouldly;
using SubscriptionManager.Application.DTOs.Invoices;
using SubscriptionManager.Application.Validators;

namespace Validators.Tests.Invoices;

public class PayInvoiceRequestValidatorTests
{
    [Fact]
    public void Success()
    {
        var request = new PayInvoiceRequest(new DateTime(2026, 9, 15));

        var result = new PayInvoiceRequestValidator().Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_ShouldAcceptAnOmittedDate()
    {
        var result = new PayInvoiceRequestValidator().Validate(new PayInvoiceRequest());

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPaidAtIsInformedButEmpty()
    {
        var request = new PayInvoiceRequest(default(DateTime));

        var result = new PayInvoiceRequestValidator().Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "A data do pagamento é inválida.");
    }
}
