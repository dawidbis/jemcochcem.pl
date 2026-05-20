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

    public DbSet<MealPlan> MealPlans { get; set; }
    public DbSet<MealPlanItem> MealPlanItems { get; set; }

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

        modelBuilder.Entity<MealPlan>()
            .HasMany(mp => mp.Items)
            .WithOne(mpi => mpi.MealPlan)
            .HasForeignKey(mpi => mpi.MealPlanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
