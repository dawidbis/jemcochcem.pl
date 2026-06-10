using FitApp.Domain.Interfaces;
using FitApp.Infrastructure.Interfaces;
using MediatR;

namespace FitApp.Application.Features.Workouts.Queries;

public record GetExerciseProgressionQuery(Guid UserId, Guid ExerciseId) : IRequest<ExerciseProgressionDto?>;

public record ExerciseProgressionDto(
    Guid ExerciseId,
    string ExerciseName,
    List<ProgressionPointDto> History,
    ProgressionSuggestionDto? Suggestion
);

public record ProgressionPointDto(
    DateTime Date,
    decimal MaxWeight,
    int BestReps,
    decimal EstOneRepMax,
    decimal TotalVolume
);

public record ProgressionSuggestionDto(
    decimal SuggestedWeight,
    int SuggestedReps,
    string Message
);

public class GetExerciseProgressionQueryHandler : IRequestHandler<GetExerciseProgressionQuery, ExerciseProgressionDto?>
{
    private readonly IWorkoutRepository _repo;
    private readonly IWorkoutCalculationService _calc;

    public GetExerciseProgressionQueryHandler(IWorkoutRepository repo, IWorkoutCalculationService calc)
    {
        _repo = repo;
        _calc = calc;
    }

    public async Task<ExerciseProgressionDto?> Handle(GetExerciseProgressionQuery request, CancellationToken cancellationToken)
    {
        var sessions = await _repo.GetSessionsByExerciseAsync(request.UserId, request.ExerciseId);
        if (sessions.Count == 0) return null;

        var exerciseName = sessions
            .SelectMany(s => s.Sets)
            .Where(s => s.ExerciseId == request.ExerciseId)
            .Select(s => s.Exercise?.Name)
            .FirstOrDefault(n => n != null) ?? "Ćwiczenie";

        var history = sessions.Select(session =>
        {
            var sets = session.Sets.Where(s => s.ExerciseId == request.ExerciseId).ToList();
            var best = sets.OrderByDescending(s => s.Weight).ThenByDescending(s => s.Reps).First();
            var orm = _calc.EstimateOneRepMax(best.Weight, best.Reps);
            var vol = sets.Sum(s => s.Weight * s.Reps);
            return new ProgressionPointDto(session.Date, best.Weight, best.Reps, orm, vol);
        })
        .OrderBy(p => p.Date)
        .ToList();

        var last = history.Last();
        decimal nextWeight;
        int nextReps;
        string msg;

        if (last.BestReps >= 10)
        {
            nextWeight = last.MaxWeight + 5m;
            nextReps = last.BestReps;
            msg = $"Świetnie! Czas na większy ciężar: {nextWeight}kg × {nextReps}";
        }
        else if (last.BestReps >= 5)
        {
            nextWeight = last.MaxWeight + 2.5m;
            nextReps = last.BestReps;
            msg = $"Dobry postęp! Spróbuj: {nextWeight}kg × {nextReps}";
        }
        else
        {
            nextWeight = last.MaxWeight;
            nextReps = last.BestReps + 1;
            msg = $"Zostań przy {last.MaxWeight}kg, dodaj powtórzenie: {nextWeight}kg × {nextReps}";
        }

        return new ExerciseProgressionDto(
            request.ExerciseId,
            exerciseName,
            history,
            new ProgressionSuggestionDto(nextWeight, nextReps, msg)
        );
    }
}
