using FitApp.Infrastructure.Interfaces;
using MediatR;

namespace FitApp.Application.Features.Workouts.Commands;

public record DeleteCustomExerciseCommand(Guid UserId, Guid ExerciseId) : IRequest<bool>;

public class DeleteCustomExerciseCommandHandler : IRequestHandler<DeleteCustomExerciseCommand, bool>
{
    private readonly IWorkoutRepository _repo;
    public DeleteCustomExerciseCommandHandler(IWorkoutRepository repo) => _repo = repo;

    public async Task<bool> Handle(DeleteCustomExerciseCommand request, CancellationToken cancellationToken)
        => await _repo.DeleteExerciseAsync(request.UserId, request.ExerciseId);
}
