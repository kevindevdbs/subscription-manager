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
    private readonly PayInvoiceHandler _payHandler;
    private readonly MarkInvoiceAsOverdueHandler _markAsOverdueHandler;
    private readonly RefundInvoiceHandler _refundHandler;
    private readonly CancelInvoiceHandler _cancelHandler;

    public InvoicesController(
        GenerateMonthlyInvoicesHandler handler,
        ListInvoicesHandler listHandler,
        PayInvoiceHandler payHandler,
        MarkInvoiceAsOverdueHandler markAsOverdueHandler,
        RefundInvoiceHandler refundHandler,
        CancelInvoiceHandler cancelHandler)
    {
        _handler = handler;
        _listHandler = listHandler;
        _payHandler = payHandler;
        _markAsOverdueHandler = markAsOverdueHandler;
        _refundHandler = refundHandler;
        _cancelHandler = cancelHandler;
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

    [HttpPatch("{id:guid}/pay")]
    [ProducesResponseType(typeof(InvoiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Pay(Guid id, [FromBody] PayInvoiceRequest? request = null)
    {
        var invoice = await _payHandler.Handle(id, request);

        return Ok(invoice);
    }

    [HttpPatch("{id:guid}/overdue")]
    [ProducesResponseType(typeof(InvoiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> MarkAsOverdue(Guid id, [FromBody] MarkInvoiceAsOverdueRequest? request = null)
    {
        var invoice = await _markAsOverdueHandler.Handle(id, request);

        return Ok(invoice);
    }

    [HttpPatch("{id:guid}/refund")]
    [ProducesResponseType(typeof(InvoiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Refund(Guid id)
    {
        var invoice = await _refundHandler.Handle(id);

        return Ok(invoice);
    }

    [HttpPatch("{id:guid}/cancel")]
    [ProducesResponseType(typeof(InvoiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var invoice = await _cancelHandler.Handle(id);

        return Ok(invoice);
    }
}
