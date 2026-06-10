namespace FitApp.Infrastructure.Interfaces;

using FitApp.Domain.Entities;

public interface IWorkoutRepository
{
    // Ćwiczenia: globalne (UserId == null) + własne danego użytkownika
    Task<List<Exercise>> GetExercisesAsync(Guid userId);
    Task AddExerciseAsync(Exercise exercise);

    Task<bool> DeleteExerciseAsync(Guid userId, Guid exerciseId);

    // Sesje treningowe
    Task AddSessionAsync(WorkoutSession session);
    Task<List<WorkoutSession>> GetSessionsAsync(Guid userId, DateTime? from, DateTime? to);
    Task<List<WorkoutSession>> GetSessionsByExerciseAsync(Guid userId, Guid exerciseId);
    Task<bool> DeleteSessionAsync(Guid userId, Guid sessionId);

    Task SaveChangesAsync();
}
