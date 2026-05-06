namespace FitApp.Infrastructure.Data.Repositories;

using FitApp.Domain.Entities;
using FitApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class FoodRepository : GenericRepository<FoodProduct>, IFoodRepository
{
    public FoodRepository(AppDbContext context) : base(context) { }

    public async Task<FoodProduct?> GetByBarcodeAsync(string barcode)
    {
        return await _dbSet.FirstOrDefaultAsync(f => f.Barcode == barcode);
    }
}