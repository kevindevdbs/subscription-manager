using Microsoft.AspNetCore.Mvc;
using SubscriptionManager.Application.DTOs.Invoices;
using SubscriptionManager.Application.UseCases.Invoices;

namespace SubscriptionManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly GenerateMonthlyInvoicesHandler _handler;

    public InvoicesController(GenerateMonthlyInvoicesHandler handler)
    {
        _handler = handler;
    }
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateMonthlyInvoices(GenerateInvoiceRequest request)
    {
        var result = await _handler.Handle(request);
        return Ok(new { generated = result });
    }
}
