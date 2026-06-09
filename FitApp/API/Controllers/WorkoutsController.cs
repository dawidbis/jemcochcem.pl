using MediatR;
using Microsoft.AspNetCore.Mvc;
using FitApp.API.Controllers;

public class WorkoutsController : ApiControllerBase
{
    private readonly IMediator _mediator;
    public WorkoutsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("exercises")]
    public async Task<IActionResult> GetExercises()
    {
        var result = await _mediator.Send(new GetExercisesQuery(CurrentUserId));
        return Ok(result);
    }

    [HttpPost("sessions")]
    public async Task<IActionResult> LogSession([FromBody] LogWorkoutSessionRequest request)
    {
        var sets = request.Sets
            .Select(s => new SetInput(s.ExerciseId, s.SetNumber, s.Weight, s.Reps))
            .ToList();

        var id = await _mediator.Send(new LogWorkoutSessionCommand(
            CurrentUserId, request.Date, request.Notes, request.DurationMinutes, sets));

        return Ok(new { sessionId = id });
    }

    [HttpGet("sessions")]
    public async Task<IActionResult> GetHistory([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var result = await _mediator.Send(new GetWorkoutHistoryQuery(CurrentUserId, from, to));
        return Ok(result);
    }
}

public record LogWorkoutSessionRequest(
    DateTime Date, string? Notes, int? DurationMinutes, List<SetItemRequest> Sets);

public record SetItemRequest(Guid ExerciseId, int SetNumber, decimal Weight, int Reps);