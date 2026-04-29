using System; 
using Microsoft.EntityFrameworkCore; 

using FitApp.Domain.Entities;
using FitApp.Domain.Interfaces;

namespace FitApp.Infrastructure.Data.Repositories;
public class MealLogRepository : GenericRepository<MealLog, Guid>, IMealLogRepository
{
    public MealLogRepository(AppDbContext context) : base(context) { }

    public override async Task<MealLog?> GetByIdAsync(Guid id) => 
        await _dbSet
            .Include(l => l.Items)
            .FirstOrDefaultAsync(l => l.Id == id);
}