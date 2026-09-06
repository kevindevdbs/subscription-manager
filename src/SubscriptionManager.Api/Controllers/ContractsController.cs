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

    public ContractsController(
        CreateContractHandler handler,
        GetContractByIdHandler getByIdHandler,
        GetInvoicesByContractHandler getInvoicesHandler)
    {
        _handler = handler;
        _getByIdHandler = getByIdHandler;
        _getInvoicesHandler = getInvoicesHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ContractResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateContract(CreateContractRequest request)
    {
        var response = await _handler.Handle(request);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
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
}
