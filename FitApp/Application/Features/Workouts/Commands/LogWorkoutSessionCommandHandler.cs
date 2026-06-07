using FitApp.Domain.Entities;
using FitApp.Infrastructure.Interfaces;
using MediatR;

public class LogWorkoutSessionCommandHandler : IRequestHandler<LogWorkoutSessionCommand, Guid>
{
    private readonly IWorkoutRepository _workoutRepo;

    public LogWorkoutSessionCommandHandler(IWorkoutRepository workoutRepo) => _workoutRepo = workoutRepo;

    public async Task<Guid> Handle(LogWorkoutSessionCommand request, CancellationToken cancellationToken)
    {
        var session = new WorkoutSession(request.UserId, request.Date, request.Notes, request.DurationMinutes);

        foreach (var s in request.Sets)
        {
            session.Sets.Add(new SetEntry(s.ExerciseId, s.SetNumber, s.Weight, s.Reps));
        }

        await _workoutRepo.AddSessionAsync(session);
        await _workoutRepo.SaveChangesAsync();

        return session.Id;
    }
}
