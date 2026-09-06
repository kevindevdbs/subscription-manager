using Microsoft.AspNetCore.Mvc;
using SubscriptionManager.Application.DTOs;
using SubscriptionManager.Application.DTOs.Invoices;
using SubscriptionManager.Application.UseCases.Invoices;

namespace SubscriptionManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly GenerateMonthlyInvoicesHandler _handler;
    private readonly ListInvoicesHandler _listHandler;

    public InvoicesController(GenerateMonthlyInvoicesHandler handler, ListInvoicesHandler listHandler)
    {
        _handler = handler;
        _listHandler = listHandler;
    }

    [HttpPost("generate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GenerateMonthlyInvoices(GenerateInvoiceRequest request)
    {
        var result = await _handler.Handle(request);

        return Ok(new { generated = result });
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<InvoiceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> List([FromQuery] ListInvoicesRequest request)
    {
        var invoices = await _listHandler.Handle(request);

        return Ok(invoices);
    }
}
