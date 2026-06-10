using FitApp.Infrastructure.Interfaces;
using MediatR;

namespace FitApp.Application.Features.Workouts.Commands;

public record DeleteWorkoutSessionCommand(Guid UserId, Guid SessionId) : IRequest<bool>;

public class DeleteWorkoutSessionCommandHandler : IRequestHandler<DeleteWorkoutSessionCommand, bool>
{
    private readonly IWorkoutRepository _repo;
    public DeleteWorkoutSessionCommandHandler(IWorkoutRepository repo) => _repo = repo;

    public async Task<bool> Handle(DeleteWorkoutSessionCommand request, CancellationToken cancellationToken)
        => await _repo.DeleteSessionAsync(request.UserId, request.SessionId);
}
