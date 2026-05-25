using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class WaterController : ControllerBase
{
    private readonly IMediator _mediator;
    public WaterController(IMediator mediator) => _mediator = mediator;

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus([FromQuery] Guid userId, [FromQuery] DateTime date)
    {
        var result = await _mediator.Send(new GetWaterStatusQuery(userId, date));
        return Ok(result);
    }

    [HttpPost("log")]
    public async Task<IActionResult> LogWater([FromBody] LogWaterRequest request)
    {
        await _mediator.Send(new LogWaterIntakeCommand(request.UserId, request.Date, request.AmountMl));
        return Ok();
    }
}

public record LogWaterRequest(Guid UserId, DateTime Date, int AmountMl);