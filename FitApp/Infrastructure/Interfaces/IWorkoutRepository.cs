namespace FitApp.Infrastructure.Interfaces;

using FitApp.Domain.Entities;

public interface IWorkoutRepository
{
    // Ćwiczenia: globalne (UserId == null) + własne danego użytkownika
    Task<List<Exercise>> GetExercisesAsync(Guid userId);
    Task AddExerciseAsync(Exercise exercise);

    // Sesje treningowe
    Task AddSessionAsync(WorkoutSession session);
    Task<List<WorkoutSession>> GetSessionsAsync(Guid userId, DateTime? from, DateTime? to);

    Task SaveChangesAsync();
}
