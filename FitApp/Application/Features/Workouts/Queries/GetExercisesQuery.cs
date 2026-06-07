using MediatR;

public record GetExercisesQuery(Guid UserId) : IRequest<List<ExerciseDto>>;

public record ExerciseDto(Guid Id, string Name, string MuscleGroup, bool IsCustom);
