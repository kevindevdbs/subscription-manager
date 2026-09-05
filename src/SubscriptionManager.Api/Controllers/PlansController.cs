using Microsoft.AspNetCore.Mvc;
using SubscriptionManager.Application.DTOs.Plans;
using SubscriptionManager.Application.UseCases.Plans;

namespace SubscriptionManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlansController : ControllerBase
{

    private readonly CreatePlanHandler _handler;

    public PlansController(CreatePlanHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlan(CreatePlanRequest request)
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
