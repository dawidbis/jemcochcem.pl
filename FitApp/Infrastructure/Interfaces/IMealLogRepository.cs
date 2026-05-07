namespace FitApp.Infrastructure.Interfaces;

using FitApp.Domain.Entities;
using System;
using System.Threading.Tasks;

public interface IMealLogRepository : IGenericRepository<MealLog>
{
    Task<MealLog?> GetByDateAsync(Guid userId, DateTime date);
    Task UpdateAsync(MealLog log);
}