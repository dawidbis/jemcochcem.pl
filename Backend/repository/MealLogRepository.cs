using FitApp.Domain.Entities;
using FitApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitApp.Infrastructure.Data.Repositories;

public class MealLogRepository : IMealLogRepository
{
    private readonly AppDbContext _context;

    public MealLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MealLog?> GetByIdAsync(Guid id) => 
        await _context.Logs
            .Include(l => l.Items) // <-- TA LINIJKA JEST KLUCZOWA
            .FirstOrDefaultAsync(l => l.Id == id);

    public async Task UpdateAsync(MealLog mealLog)
    {
        _context.Logs.Update(mealLog);
        await _context.SaveChangesAsync();
    }
}