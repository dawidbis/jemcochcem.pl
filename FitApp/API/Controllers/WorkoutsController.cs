using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class WorkoutsController : ControllerBase
{
    private readonly IMediator _mediator;
    public WorkoutsController(IMediator mediator) => _mediator = mediator;

    // GET /api/workouts/exercises?userId=...
    [HttpGet("exercises")]
    public async Task<IActionResult> GetExercises([FromQuery] Guid userId)
    {
        var result = await _mediator.Send(new GetExercisesQuery(userId));
        return Ok(result);
    }

    // POST /api/workouts/sessions
    [HttpPost("sessions")]
    public async Task<IActionResult> LogSession([FromBody] LogWorkoutSessionRequest request)
    {
        var sets = request.Sets
            .Select(s => new SetInput(s.ExerciseId, s.SetNumber, s.Weight, s.Reps))
            .ToList();

        var id = await _mediator.Send(new LogWorkoutSessionCommand(
            request.UserId, request.Date, request.Notes, request.DurationMinutes, sets));

        return Ok(new { sessionId = id });
    }

    // GET /api/workouts/sessions?userId=...&from=...&to=...
    [HttpGet("sessions")]
    public async Task<IActionResult> GetHistory([FromQuery] Guid userId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var result = await _mediator.Send(new GetWorkoutHistoryQuery(userId, from, to));
        return Ok(result);
    }
}

public record LogWorkoutSessionRequest(
    Guid UserId,
    DateTime Date,
    string? Notes,
    int? DurationMinutes,
    List<SetItemRequest> Sets);

public record SetItemRequest(Guid ExerciseId, int SetNumber, decimal Weight, int Reps);
