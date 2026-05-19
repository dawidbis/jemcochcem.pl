using FitApp.Domain.Entities;
using FitApp.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitApp.Infrastructure.Data.Repositories;

public class MealPlanRepository : IMealPlanRepository
{
    private readonly AppDbContext _context;

    public MealPlanRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MealPlan> AddAsync(MealPlan mealPlan)
    {
        await _context.MealPlans.AddAsync(mealPlan);
        // Jeśli nie używasz Unit of Work, pamiętaj o SaveChanges
        await _context.SaveChangesAsync(); 
        return mealPlan;
    }

    public async Task<IEnumerable<MealPlan>> GetByUserIdAsync(Guid userId)
    {
        return await _context.MealPlans
            .Include(mp => mp.Items)
            .Where(mp => mp.UserId == userId)
            .OrderByDescending(mp => mp.CreatedAt)
            .ToListAsync();
    }

    public async Task<MealPlanItem?> GetItemByIdAsync(Guid id)
    {
        return await _context.MealPlanItems
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task UpdateItemAsync(MealPlanItem item)
    {
        _context.MealPlanItems.Update(item);
        await _context.SaveChangesAsync();
    }
}