using FitApp.Domain.Entities;

namespace FitApp.Domain.Interfaces;

public interface IFoodRepository
{
    Task<FoodProduct?> GetByBarcodeAsync(string code);
    Task AddAsync(FoodProduct product);
}