using FitApp.Infrastructure.Interfaces;
using MediatR;

public class GetExercisesQueryHandler : IRequestHandler<GetExercisesQuery, List<ExerciseDto>>
{
    private readonly IWorkoutRepository _workoutRepo;

    public GetExercisesQueryHandler(IWorkoutRepository workoutRepo) => _workoutRepo = workoutRepo;

    public async Task<List<ExerciseDto>> Handle(GetExercisesQuery request, CancellationToken cancellationToken)
    {
        var exercises = await _workoutRepo.GetExercisesAsync(request.UserId);

        return exercises
            .Select(e => new ExerciseDto(e.Id, e.Name, e.MuscleGroup, e.IsCustom))
            .ToList();
    }
}
