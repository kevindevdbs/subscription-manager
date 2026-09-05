using Microsoft.AspNetCore.Mvc;
using SubscriptionManager.Application.DTOs.Customers;
using SubscriptionManager.Application.UseCases.Customers;

namespace SubscriptionManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{

    private readonly CreateCustomerHandler _handler;

    public CustomersController(CreateCustomerHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer(CreateCustomerRequest request)
    {
        var response = await _handler.Handle(request);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        return NotFound();
    }
}
