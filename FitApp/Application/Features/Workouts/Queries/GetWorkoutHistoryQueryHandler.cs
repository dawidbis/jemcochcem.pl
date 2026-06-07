using FitApp.Domain.Interfaces;
using FitApp.Infrastructure.Interfaces;
using MediatR;

public class GetWorkoutHistoryQueryHandler : IRequestHandler<GetWorkoutHistoryQuery, List<WorkoutSessionDto>>
{
    private readonly IWorkoutRepository _workoutRepo;
    private readonly IWorkoutCalculationService _calc;

    public GetWorkoutHistoryQueryHandler(IWorkoutRepository workoutRepo, IWorkoutCalculationService calc)
    {
        _workoutRepo = workoutRepo;
        _calc = calc;
    }

    public async Task<List<WorkoutSessionDto>> Handle(GetWorkoutHistoryQuery request, CancellationToken cancellationToken)
    {
        var sessions = await _workoutRepo.GetSessionsAsync(request.UserId, request.From, request.To);

        return sessions.Select(s =>
        {
            var sets = s.Sets
                .OrderBy(se => se.Exercise != null ? se.Exercise.Name : string.Empty)
                .ThenBy(se => se.SetNumber)
                .Select(se => new WorkoutSetDto(
                    se.ExerciseId,
                    se.Exercise?.Name ?? "Nieznane ćwiczenie",
                    se.Exercise?.MuscleGroup ?? "",
                    se.SetNumber,
                    se.Weight,
                    se.Reps))
                .ToList();

            // Logika domenowa wydzielona do serwisu (spójnie z NutritionCalculationService)
            var totalVolume = _calc.CalculateSessionVolume(s.Sets);

            return new WorkoutSessionDto(
                s.Id,
                s.Date,
                s.Notes,
                s.DurationMinutes,
                sets.Count,
                totalVolume,
                sets);
        }).ToList();
    }
}
