using FitApp.Domain.Entities;
using FitApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitApp.Infrastructure.Data.Repositories;

public class FoodRepository : IFoodRepository
{
    private readonly AppDbContext _context;

    public FoodRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<FoodProduct?> GetByBarcodeAsync(string code) => 
        await _context.Products.FirstOrDefaultAsync(p => p.Barcode == code);

    public async Task AddAsync(FoodProduct product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }
}