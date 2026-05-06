namespace FitApp.Domain.Interfaces;

using FitApp.Domain.Entities;
using System.Threading.Tasks;

public interface IFoodRepository : IGenericRepository<FoodProduct>
{
    Task<FoodProduct?> GetByBarcodeAsync(string barcode);
}