using Microsoft.AspNetCore.Mvc;
using SubscriptionManager.Application.DTOs;
using SubscriptionManager.Application.DTOs.Plans;
using SubscriptionManager.Application.UseCases.Plans;

namespace SubscriptionManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlansController : ControllerBase
{

    private readonly CreatePlanHandler _handler;
    private readonly GetPlanByIdHandler _getByIdHandler;

    public PlansController(CreatePlanHandler handler, GetPlanByIdHandler getByIdHandler)
    {
        _handler = handler;
        _getByIdHandler = getByIdHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PlanResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePlan(CreatePlanRequest request)
    {
        var response = await _handler.Handle(request);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PlanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _getByIdHandler.Handle(id);

        return Ok(response);
    }
}
