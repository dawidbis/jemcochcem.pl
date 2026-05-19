namespace FitApp.Infrastructure.Data;

using FitApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<MealLog> MealLogs { get; set; } = null!;
    public DbSet<FoodProduct> FoodProducts { get; set; } = null!;
    public DbSet<BodyMeasurement> BodyMeasurements { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BodyMeasurement>()
            .Property(b => b.Weight).HasPrecision(5, 2);

        modelBuilder.Entity<BodyMeasurement>()
            .Property(b => b.Waist).HasPrecision(5, 2);

        modelBuilder.Entity<BodyMeasurement>()
            .Property(b => b.Hips).HasPrecision(5, 2);

        modelBuilder.Entity<User>()
            .Property(u => u.TargetWeight).HasPrecision(5, 2);
    }
}
