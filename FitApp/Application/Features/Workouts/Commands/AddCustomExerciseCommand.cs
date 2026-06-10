using FitApp.Domain.Entities;
using FitApp.Infrastructure.Interfaces;
using MediatR;

namespace FitApp.Application.Features.Workouts.Commands;

public record AddCustomExerciseCommand(Guid UserId, string Name, string MuscleGroup) : IRequest<Guid>;

public class AddCustomExerciseCommandHandler : IRequestHandler<AddCustomExerciseCommand, Guid>
{
    private readonly IWorkoutRepository _repo;
    public AddCustomExerciseCommandHandler(IWorkoutRepository repo) => _repo = repo;

    public async Task<Guid> Handle(AddCustomExerciseCommand request, CancellationToken cancellationToken)
    {
        var exercise = new Exercise(request.Name.Trim(), request.MuscleGroup, isCustom: true, userId: request.UserId);
        await _repo.AddExerciseAsync(exercise);
        await _repo.SaveChangesAsync();
        return exercise.Id;
    }
}
