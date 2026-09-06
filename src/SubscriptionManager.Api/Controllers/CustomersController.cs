using Microsoft.AspNetCore.Mvc;
using SubscriptionManager.Application.DTOs;
using SubscriptionManager.Application.DTOs.Customers;
using SubscriptionManager.Application.UseCases.Customers;

namespace SubscriptionManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{

    private readonly CreateCustomerHandler _handler;
    private readonly GetCustomerByIdHandler _getByIdHandler;

    public CustomersController(CreateCustomerHandler handler, GetCustomerByIdHandler getByIdHandler)
    {
        _handler = handler;
        _getByIdHandler = getByIdHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateCustomer(CreateCustomerRequest request)
    {
        var response = await _handler.Handle(request);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _getByIdHandler.Handle(id);

        return Ok(response);
    }
}
