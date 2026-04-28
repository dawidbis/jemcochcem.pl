using FitApp.Domain.Entities;

namespace FitApp.Domain.Interfaces;

public interface IMealLogRepository
{
    Task<MealLog?> GetByIdAsync(Guid id);
    Task UpdateAsync(MealLog mealLog);
}