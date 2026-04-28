using FitApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FitApp.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<FoodProduct> Products { get; set; }
    public DbSet<MealLog> Logs { get; set; }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // Ustawienie Barcode jako klucza głównego dla FoodProduct
        mb.Entity<FoodProduct>().HasKey(f => f.Barcode);
        
        // Kolega rozszerzy OnModelCreating o relacje encji w punkcie 5.
        base.OnModelCreating(mb);
    }
}