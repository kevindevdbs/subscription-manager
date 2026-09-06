using Shouldly;
using SubscriptionManager.Application.DTOs.Invoices;
using SubscriptionManager.Application.Validators;

namespace Validators.Tests.Invoices;

public class ListInvoicesRequestValidatorTests
{
    [Fact]
    public void Success_WhenNoFilterIsInformed()
    {
        var result = new ListInvoicesRequestValidator().Validate(new ListInvoicesRequest(null, null));

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("Pending")]
    [InlineData("Overdue")]
    [InlineData("pending")]
    [InlineData("OVERDUE")]
    public void Success_WhenStatusIsValid(string status)
    {
        var result = new ListInvoicesRequestValidator().Validate(new ListInvoicesRequest(status, null));

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("2026-09")]
    [InlineData("2025-12")]
    public void Success_WhenMonthIsValid(string month)
    {
        var result = new ListInvoicesRequestValidator().Validate(new ListInvoicesRequest(null, month));

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("xpto")]
    [InlineData("Atrasada")]
    [InlineData("99")]
    public void Validate_ShouldHaveError_WhenStatusIsInvalid(string status)
    {
        var result = new ListInvoicesRequestValidator().Validate(new ListInvoicesRequest(status, null));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage.StartsWith("Status inválido."));
    }

    [Theory]
    [InlineData("setembro")]
    [InlineData("2026-13")]
    [InlineData("09-2026")]
    [InlineData("2026")]
    public void Validate_ShouldHaveError_WhenMonthIsInvalid(string month)
    {
        var result = new ListInvoicesRequestValidator().Validate(new ListInvoicesRequest(null, month));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage.StartsWith("Mês inválido."));
    }

    [Fact]
    public void Validate_ShouldReportBothErrors_WhenStatusAndMonthAreInvalid()
    {
        var result = new ListInvoicesRequestValidator().Validate(new ListInvoicesRequest("xpto", "setembro"));

        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(2);
    }
}
