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
    private readonly ListPlansHandler _listHandler;
    private readonly DeactivatePlanHandler _deactivateHandler;

    public PlansController(
        CreatePlanHandler handler,
        GetPlanByIdHandler getByIdHandler,
        ListPlansHandler listHandler,
        DeactivatePlanHandler deactivateHandler)
    {
        _handler = handler;
        _getByIdHandler = getByIdHandler;
        _listHandler = listHandler;
        _deactivateHandler = deactivateHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PlanResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePlan(CreatePlanRequest request)
    {
        var response = await _handler.Handle(request);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PlanResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List()
    {
        var plans = await _listHandler.Handle();

        return Ok(plans);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PlanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _getByIdHandler.Handle(id);

        return Ok(response);
    }

    /// <summary>
    /// Descontinua o plano. Contratos existentes seguem sendo faturados;
    /// o plano só deixa de aceitar adesão nova.
    /// </summary>
    [HttpPatch("{id:guid}/deactivate")]
    [ProducesResponseType(typeof(PlanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var response = await _deactivateHandler.Handle(id);

        return Ok(response);
    }
}
