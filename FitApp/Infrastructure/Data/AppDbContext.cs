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
    public DbSet<WaterLog> WaterLogs { get; set; }
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<WorkoutSession> WorkoutSessions { get; set; }
    public DbSet<SetEntry> SetEntries { get; set; }
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
    
    
        // --- MODUŁ TRENINGOWY ---
        modelBuilder.Entity<SetEntry>()
            .Property(s => s.Weight).HasPrecision(6, 2);

        modelBuilder.Entity<WorkoutSession>()
            .HasMany(s => s.Sets)
            .WithOne(se => se.WorkoutSession)
            .HasForeignKey(se => se.WorkoutSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SetEntry>()
            .HasOne(se => se.Exercise)
            .WithMany()
            .HasForeignKey(se => se.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Exercise>().HasData(
            new Exercise { Id = Guid.Parse("9eb50c5b-8614-4b2c-bc2f-4bcab75e9989"), Name = "Wyciskanie sztangi leżąc", MuscleGroup = "Klatka", IsCustom = false, UserId = null },
            new Exercise { Id = Guid.Parse("d1200c1c-082a-45f2-b1c3-d16db2541031"), Name = "Wyciskanie hantli na skosie", MuscleGroup = "Klatka", IsCustom = false, UserId = null },
            new Exercise { Id = Guid.Parse("b1150c95-cda4-493d-b637-48f51de164df"), Name = "Rozpiętki na bramie", MuscleGroup = "Klatka", IsCustom = false, UserId = null },
            new Exercise { Id = Guid.Parse("75941a11-48c6-4853-8b94-b8b078cfdf9f"), Name = "Martwy ciąg", MuscleGroup = "Plecy", IsCustom = false, UserId = null },
            new Exercise { Id = Guid.Parse("0755d1d9-54d5-48bb-82ac-8ad58029ba4c"), Name = "Podciąganie na drążku", MuscleGroup = "Plecy", IsCustom = false, UserId = null },
            new Exercise { Id = Guid.Parse("9e74f916-fb5a-49f6-b02d-039f2d874208"), Name = "Wiosłowanie sztangą", MuscleGroup = "Plecy", IsCustom = false, UserId = null },
            new Exercise { Id = Guid.Parse("a21bbae5-9d73-48d5-8b72-98a555183993"), Name = "Przysiad ze sztangą", MuscleGroup = "Nogi", IsCustom = false, UserId = null },
            new Exercise { Id = Guid.Parse("900179c0-e035-41c8-8cff-41047d53f728"), Name = "Wykroki z hantlami", MuscleGroup = "Nogi", IsCustom = false, UserId = null },
            new Exercise { Id = Guid.Parse("36cf2c4b-31ae-4ca8-8578-fa485486b3ec"), Name = "Prostowanie nóg na maszynie", MuscleGroup = "Nogi", IsCustom = false, UserId = null },
            new Exercise { Id = Guid.Parse("0b529b22-6546-4522-8c7c-dbda75485c28"), Name = "Wyciskanie żołnierskie (OHP)", MuscleGroup = "Barki", IsCustom = false, UserId = null },
            new Exercise { Id = Guid.Parse("5d808eac-6a21-4a6a-b85c-3a97d0d38faf"), Name = "Wznosy bokiem", MuscleGroup = "Barki", IsCustom = false, UserId = null },
            new Exercise { Id = Guid.Parse("52ed863f-f00c-4de8-8a27-035cf89faeea"), Name = "Uginanie ramion ze sztangą", MuscleGroup = "Ramiona", IsCustom = false, UserId = null },
            new Exercise { Id = Guid.Parse("9753b98f-b41a-4411-bb8b-87af642b92e8"), Name = "Prostowanie ramion na wyciągu", MuscleGroup = "Ramiona", IsCustom = false, UserId = null },
            new Exercise { Id = Guid.Parse("bb543db9-e090-4d10-ba5c-f09c5d404b8c"), Name = "Plank", MuscleGroup = "Brzuch", IsCustom = false, UserId = null },
            new Exercise { Id = Guid.Parse("7eee9d25-94a3-4c08-b47d-136aca842828"), Name = "Unoszenie nóg", MuscleGroup = "Brzuch", IsCustom = false, UserId = null },
            new Exercise { Id = Guid.Parse("3eb0f25b-2f7b-457d-a2c8-3e9824b75560"), Name = "Bieg / cardio", MuscleGroup = "Cardio", IsCustom = false, UserId = null }
        );
    }
}
