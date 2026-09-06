using Microsoft.AspNetCore.Mvc;
using SubscriptionManager.Application.DTOs.Contracts;
using SubscriptionManager.Application.UseCases.Contracts;

namespace SubscriptionManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContractsController : ControllerBase
{

    private readonly CreateContractHandler _handler;

    public ContractsController(CreateContractHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    public async Task<IActionResult> CreateContract(CreateContractRequest request)
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
