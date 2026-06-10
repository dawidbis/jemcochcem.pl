namespace FitApp.Infrastructure.Data.Repositories;

using FitApp.Domain.Entities;
using FitApp.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

    public async Task<List<MealLog>> GetMonthAsync(Guid userId, int year, int month)
    {
        var from = new DateTime(year, month, 1);
        var to = from.AddMonths(1);
        return await _dbSet
            .Where(m => m.UserId == userId && m.Date >= from && m.Date < to)
            .ToListAsync();
    }

    public async Task AddMealLogItemAsync(MealLogItem item)
    {
        await _context
            .Set<MealLogItem>()
            .AddAsync(item);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}