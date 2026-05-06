namespace FitApp.Infrastructure.Data.Repositories;

using FitApp.Domain.Entities;
using FitApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

public class MealLogRepository : GenericRepository<MealLog>, IMealLogRepository
{
    public MealLogRepository(AppDbContext context) : base(context) { }

    public async Task<MealLog?> GetByDateAsync(Guid userId, DateTime date)
    {
        return await _dbSet
            .Include(m => m.Items)
            .ThenInclude(i => i.FoodProduct)
            .FirstOrDefaultAsync(m => m.UserId == userId && m.Date.Date == date.Date);
    }

    public async Task UpdateAsync(MealLog log)
    {
        _context.Entry(log).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}