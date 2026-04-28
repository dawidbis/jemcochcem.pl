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
        
        // --- RELACJE Z PUNKTU 5 ---

        // 1. Konfiguracja tzw. Value Objects (obiekty bez własnego ID, osadzone w innych tabelach)
        mb.Entity<User>().OwnsOne(u => u.Goal);
        mb.Entity<FoodProduct>().OwnsOne(f => f.MacrosPer100g);
        mb.Entity<MealLogItem>().OwnsOne(i => i.CalculatedMacros);

        // 2. Relacja jeden-do-wielu: Dziennik posiłków (1) -> Pozycje w dzienniku (Wiele)
        mb.Entity<MealLog>()
            .HasMany(m => m.Items)
            .WithOne()
            .HasForeignKey("MealLogId");

        base.OnModelCreating(mb);
    }
}