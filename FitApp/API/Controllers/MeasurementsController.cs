using FitApp.Application.Features.Diet;
using FitApp.Application.Features.Measurements;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitApp.API.Controllers;

public class MeasurementsController : ApiControllerBase
{
    private readonly IMediator _mediator;
    public MeasurementsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMeasurementRequest request)
    {
        var command = new CreateMeasurementCommand(
            CurrentUserId, request.Weight, request.Date,
            request.BodyFatPercentage, request.Waist, request.Hips, request.Notes);
        var id = await _mediator.Send(command);
        return Ok(new { Id = id });
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetHistory()
    {
        var result = await _mediator.Send(new GetMeasurementsQuery(CurrentUserId));
        return Ok(result);
    }

    [HttpGet("me/stats")]
    public async Task<IActionResult> GetStats()
    {
        var result = await _mediator.Send(new GetMeasurementStatsQuery(CurrentUserId));
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteMeasurementCommand(id));
        return result ? NoContent() : NotFound();
    }

    public record CreateMeasurementRequest(
        decimal Weight, DateTime Date,
        decimal? BodyFatPercentage = null, decimal? Waist = null,
        decimal? Hips = null, string? Notes = null);
}