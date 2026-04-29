using FitApp.Domain.Entities;

namespace FitApp.Domain.Interfaces;

// Przyjmuję int jako TId, jeśli używasz Guid, zamień int na Guid
public interface IFoodRepository : IGenericRepository<FoodProduct, int>
{
    Task<FoodProduct?> GetByBarcodeAsync(string code);
}