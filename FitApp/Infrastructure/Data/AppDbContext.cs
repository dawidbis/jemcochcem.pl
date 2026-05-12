namespace FitApp.Infrastructure.Data;

using FitApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<MealLog> MealLogs { get; set; } = null!;
    public DbSet<FoodProduct> FoodProducts { get; set; } = null!;
    public DbSet<BodyMeasurement> BodyMeasurements { get; set; } = null!; // Dodano

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Opcjonalna konfiguracja precyzji dla decimal (ważne w FitApp)
        modelBuilder.Entity<BodyMeasurement>()
            .Property(b => b.Weight).HasPrecision(5, 2);
    }
}