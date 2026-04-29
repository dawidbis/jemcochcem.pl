using System.Threading.Tasks;
using FitApp.Domain.Entities;
using FitApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitApp.Infrastructure.Data.Repositories;

public class FoodRepository : GenericRepository<FoodProduct, int>, IFoodRepository
{
    public FoodRepository(AppDbContext context) : base(context) 
    { 
    }

    public async Task<FoodProduct?> GetByBarcodeAsync(string code)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Barcode == code);
    }
}