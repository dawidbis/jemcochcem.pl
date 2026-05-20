using FitApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitApp.Infrastructure.Interfaces;

// Jeśli używasz IGenericRepository, możesz po nim dziedziczyć: 
// public interface IMealPlanRepository : IGenericRepository<MealPlan>
public interface IMealPlanRepository 
{
    Task<MealPlan> AddAsync(MealPlan mealPlan);
    Task<IEnumerable<MealPlan>> GetByUserIdAsync(Guid userId);
    Task<MealPlanItem?> GetItemByIdAsync(Guid id);
    Task UpdateItemAsync(MealPlanItem item);
}