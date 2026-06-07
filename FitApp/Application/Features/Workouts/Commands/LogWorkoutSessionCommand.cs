using MediatR;

// Pojedyncza seria przesyłana z frontu
public record SetInput(Guid ExerciseId, int SetNumber, decimal Weight, int Reps);

public record LogWorkoutSessionCommand(
    Guid UserId,
    DateTime Date,
    string? Notes,
    int? DurationMinutes,
    List<SetInput> Sets
) : IRequest<Guid>;
