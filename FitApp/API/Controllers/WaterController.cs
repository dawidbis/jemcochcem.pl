using MediatR;
using Microsoft.AspNetCore.Mvc;
using FitApp.API.Controllers;

public class WaterController : ApiControllerBase
{
    private readonly IMediator _mediator;
    public WaterController(IMediator mediator) => _mediator = mediator;

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus([FromQuery] DateTime date)
    {
        var result = await _mediator.Send(new GetWaterStatusQuery(CurrentUserId, date));
        return Ok(result);
    }

    [HttpPost("log")]
    public async Task<IActionResult> LogWater([FromBody] LogWaterRequest request)
    {
        await _mediator.Send(new LogWaterIntakeCommand(CurrentUserId, request.Date, request.AmountMl));
        return Ok();
    }
}

public record LogWaterRequest(DateTime Date, int AmountMl);