using FitApp.Application.Features.Diet;
using FitApp.Application.Features.Measurements;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeasurementsController : ControllerBase
{
    private readonly IMediator _mediator;

    public MeasurementsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMeasurementCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { Id = id });
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetHistory(Guid userId)
    {
        var result = await _mediator.Send(new GetMeasurementsQuery(userId));
        return Ok(result);
    }

    [HttpGet("user/{userId}/stats")]
    public async Task<IActionResult> GetStats(Guid userId)
    {
        var result = await _mediator.Send(new GetMeasurementStatsQuery(userId));
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteMeasurementCommand(id));
        return result ? NoContent() : NotFound();
    }
}
