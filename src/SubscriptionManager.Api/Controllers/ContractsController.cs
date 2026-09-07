using Microsoft.AspNetCore.Mvc;
using SubscriptionManager.Application.DTOs;
using SubscriptionManager.Application.DTOs.Contracts;
using SubscriptionManager.Application.DTOs.Invoices;
using SubscriptionManager.Application.UseCases.Contracts;
using SubscriptionManager.Application.UseCases.Invoices;

namespace SubscriptionManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContractsController : ControllerBase
{

    private readonly CreateContractHandler _handler;
    private readonly GetContractByIdHandler _getByIdHandler;
    private readonly GetInvoicesByContractHandler _getInvoicesHandler;
    private readonly ListContractsHandler _listHandler;
    private readonly SuspendContractHandler _suspendHandler;
    private readonly ReactivateContractHandler _reactivateHandler;
    private readonly CancelContractHandler _cancelHandler;

    public ContractsController(
        CreateContractHandler handler,
        GetContractByIdHandler getByIdHandler,
        GetInvoicesByContractHandler getInvoicesHandler,
        ListContractsHandler listHandler,
        SuspendContractHandler suspendHandler,
        ReactivateContractHandler reactivateHandler,
        CancelContractHandler cancelHandler)
    {
        _handler = handler;
        _getByIdHandler = getByIdHandler;
        _getInvoicesHandler = getInvoicesHandler;
        _listHandler = listHandler;
        _suspendHandler = suspendHandler;
        _reactivateHandler = reactivateHandler;
        _cancelHandler = cancelHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ContractResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateContract(CreateContractRequest request)
    {
        var response = await _handler.Handle(request);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ContractResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List()
    {
        var contracts = await _listHandler.Handle();

        return Ok(contracts);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ContractResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _getByIdHandler.Handle(id);

        return Ok(response);
    }

    [HttpGet("{id:guid}/invoices")]
    [ProducesResponseType(typeof(IEnumerable<InvoiceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInvoices(Guid id)
    {
        var invoices = await _getInvoicesHandler.Handle(id);

        return Ok(invoices);
    }

    [HttpPatch("{id:guid}/suspend")]
    [ProducesResponseType(typeof(ContractResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Suspend(Guid id)
    {
        var contract = await _suspendHandler.Handle(id);

        return Ok(contract);
    }

    [HttpPatch("{id:guid}/reactivate")]
    [ProducesResponseType(typeof(ContractResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Reactivate(Guid id)
    {
        var contract = await _reactivateHandler.Handle(id);

        return Ok(contract);
    }

    [HttpPatch("{id:guid}/cancel")]
    [ProducesResponseType(typeof(ContractResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Cancel(Guid id, CancelContractRequest request)
    {
        var contract = await _cancelHandler.Handle(id, request);

        return Ok(contract);
    }
}
