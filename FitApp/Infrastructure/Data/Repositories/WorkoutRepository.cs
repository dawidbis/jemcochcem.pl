using FitApp.Domain.Entities;
using FitApp.Infrastructure.Data;
using FitApp.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitApp.Infrastructure.Repositories
{
    public class WorkoutRepository : IWorkoutRepository
    {
        private readonly AppDbContext _context;

        public WorkoutRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Exercise>> GetExercisesAsync(Guid userId)
        {
            // Globalne (UserId == null) + ćwiczenia własne danego użytkownika
            return await _context.Exercises
                .Where(e => e.UserId == null || e.UserId == userId)
                .OrderBy(e => e.MuscleGroup)
                .ThenBy(e => e.Name)
                .ToListAsync();
        }

        public async Task AddExerciseAsync(Exercise exercise)
        {
            await _context.Exercises.AddAsync(exercise);
        }

        public async Task AddSessionAsync(WorkoutSession session)
        {
            await _context.WorkoutSessions.AddAsync(session);
        }

        public async Task<List<WorkoutSession>> GetSessionsAsync(Guid userId, DateTime? from, DateTime? to)
        {
            var query = _context.WorkoutSessions
                .Include(s => s.Sets)
                    .ThenInclude(se => se.Exercise)
                .Where(s => s.UserId == userId);

            if (from.HasValue)
            {
                var f = from.Value.Date;
                query = query.Where(s => s.Date >= f);
            }

            if (to.HasValue)
            {
                var t = to.Value.Date;
                query = query.Where(s => s.Date <= t);
            }

            return await query
                .OrderByDescending(s => s.Date)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
