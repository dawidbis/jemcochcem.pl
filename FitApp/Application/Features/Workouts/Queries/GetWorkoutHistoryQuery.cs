using MediatR;

public record GetWorkoutHistoryQuery(Guid UserId, DateTime? From, DateTime? To) : IRequest<List<WorkoutSessionDto>>;

public record WorkoutSetDto(
    Guid ExerciseId,
    string ExerciseName,
    string MuscleGroup,
    int SetNumber,
    decimal Weight,
    int Reps
);

public record WorkoutSessionDto(
    Guid Id,
    DateTime Date,
    string? Notes,
    int? DurationMinutes,
    int TotalSets,
    decimal TotalVolume,        // suma (ciężar * powtórzenia) – tonaż sesji
    List<WorkoutSetDto> Sets
);
