namespace FitApp.Infrastructure.Data;

using FitApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public static class DataSeeder
{
    // ── Fixed IDs (stałe GUIDy → idempotentny seeder) ──────────────────────────
    static readonly Guid UserId = Guid.Parse("d0e1f2a3-b4c5-6789-abcd-ef0123456789");

    // Produkty spożywcze
    static readonly Guid FChicken  = Guid.Parse("a0000001-feed-0000-0000-000000000001");
    static readonly Guid FRice     = Guid.Parse("a0000001-feed-0000-0000-000000000002");
    static readonly Guid FOats     = Guid.Parse("a0000001-feed-0000-0000-000000000003");
    static readonly Guid FBanana   = Guid.Parse("a0000001-feed-0000-0000-000000000004");
    static readonly Guid FCottage  = Guid.Parse("a0000001-feed-0000-0000-000000000005");
    static readonly Guid FYogurt   = Guid.Parse("a0000001-feed-0000-0000-000000000006");
    static readonly Guid FBroccoli = Guid.Parse("a0000001-feed-0000-0000-000000000007");
    static readonly Guid FBread    = Guid.Parse("a0000001-feed-0000-0000-000000000008");

    // Ćwiczenia (z migracji AddWorkoutModule)
    static readonly Guid ExBench    = Guid.Parse("9eb50c5b-8614-4b2c-bc2f-4bcab75e9989");
    static readonly Guid ExOhp      = Guid.Parse("0b529b22-6546-4522-8c7c-dbda75485c28");
    static readonly Guid ExDeadlift = Guid.Parse("75941a11-48c6-4853-8b94-b8b078cfdf9f");
    static readonly Guid ExRows     = Guid.Parse("9e74f916-fb5a-49f6-b02d-039f2d874208");
    static readonly Guid ExPullups  = Guid.Parse("0755d1d9-54d5-48bb-82ac-8ad58029ba4c");
    static readonly Guid ExSquat    = Guid.Parse("a21bbae5-9d73-48d5-8b72-98a555183993");
    static readonly Guid ExLunges   = Guid.Parse("900179c0-e035-41c8-8cff-41047d53f728");
    static readonly Guid ExLegExt   = Guid.Parse("36cf2c4b-31ae-4ca8-8578-fa485486b3ec");
    static readonly Guid ExBicep    = Guid.Parse("52ed863f-f00c-4de8-8a27-035cf89faeea");
    static readonly Guid ExTricep   = Guid.Parse("9753b98f-b41a-4411-bb8b-87af642b92e8");

    // ── Wejście ─────────────────────────────────────────────────────────────────
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Users.AnyAsync()) return;

        await SeedFoodsAsync(db);
        await SeedUserAsync(db);
        await SeedMeasurementsAsync(db);
        await SeedMealLogsAsync(db);
        await SeedWaterLogsAsync(db);
        await SeedWorkoutsAsync(db);
    }

    // ── Produkty ─────────────────────────────────────────────────────────────────
    static async Task SeedFoodsAsync(AppDbContext db)
    {
        db.FoodProducts.AddRange(
            new FoodProduct { Id = FChicken,  Name = "Pierś z kurczaka (pieczona)", CaloriesPer100g = 165, ProteinPer100g = 31m,  CarbsPer100g = 0m,    FatsPer100g = 3.6m,  FiberPer100g = 0m,   SugarsPer100g = 0m,   SaturatedFatPer100g = 0.9m,  SodiumPer100g = 70m,  CalciumPer100g = 15m,  IronPer100g = 0.7m },
            new FoodProduct { Id = FRice,     Name = "Ryż biały (ugotowany)",       CaloriesPer100g = 130, ProteinPer100g = 2.7m, CarbsPer100g = 28m,   FatsPer100g = 0.3m,  FiberPer100g = 0.4m, SugarsPer100g = 0m,   SaturatedFatPer100g = 0.1m,  SodiumPer100g = 1m,   CalciumPer100g = 10m,  IronPer100g = 0.2m },
            new FoodProduct { Id = FOats,     Name = "Płatki owsiane górskie",      CaloriesPer100g = 370, ProteinPer100g = 13m,  CarbsPer100g = 60m,   FatsPer100g = 7m,    FiberPer100g = 10m,  SugarsPer100g = 1m,   SaturatedFatPer100g = 1.4m,  SodiumPer100g = 6m,   CalciumPer100g = 54m,  IronPer100g = 4m },
            new FoodProduct { Id = FBanana,   Name = "Banan świeży",                CaloriesPer100g = 89,  ProteinPer100g = 1.1m, CarbsPer100g = 23m,   FatsPer100g = 0.3m,  FiberPer100g = 2.6m, SugarsPer100g = 12m,  SaturatedFatPer100g = 0.1m,  SodiumPer100g = 1m,   CalciumPer100g = 5m,   IronPer100g = 0.3m },
            new FoodProduct { Id = FCottage,  Name = "Twaróg chudy (0%)",           CaloriesPer100g = 70,  ProteinPer100g = 12m,  CarbsPer100g = 3m,    FatsPer100g = 0.5m,  FiberPer100g = 0m,   SugarsPer100g = 3m,   SaturatedFatPer100g = 0.3m,  SodiumPer100g = 30m,  CalciumPer100g = 83m,  IronPer100g = 0.1m },
            new FoodProduct { Id = FYogurt,   Name = "Jogurt grecki (2%)",          CaloriesPer100g = 73,  ProteinPer100g = 5.7m, CarbsPer100g = 3.6m,  FatsPer100g = 3.3m,  FiberPer100g = 0m,   SugarsPer100g = 3.6m, SaturatedFatPer100g = 2m,    SodiumPer100g = 36m,  CalciumPer100g = 110m, IronPer100g = 0m },
            new FoodProduct { Id = FBroccoli, Name = "Brokuły (gotowane)",          CaloriesPer100g = 34,  ProteinPer100g = 2.8m, CarbsPer100g = 6.6m,  FatsPer100g = 0.4m,  FiberPer100g = 2.6m, SugarsPer100g = 1.7m, SaturatedFatPer100g = 0.1m,  SodiumPer100g = 41m,  CalciumPer100g = 47m,  IronPer100g = 0.7m },
            new FoodProduct { Id = FBread,    Name = "Chleb pełnoziarnisty",        CaloriesPer100g = 247, ProteinPer100g = 9m,   CarbsPer100g = 46m,   FatsPer100g = 3m,    FiberPer100g = 6m,   SugarsPer100g = 4m,   SaturatedFatPer100g = 0.5m,  SodiumPer100g = 400m, CalciumPer100g = 30m,  IronPer100g = 2.5m }
        );
        await db.SaveChangesAsync();
    }

    // ── Użytkownik demo ──────────────────────────────────────────────────────────
    static async Task SeedUserAsync(AppDbContext db)
    {
        db.Users.Add(new User
        {
            Id             = UserId,
            Email          = "demo@fitapp.pl",
            PasswordHash   = BCrypt.Net.BCrypt.HashPassword("Demo123!"),
            Weight         = 85m,
            Height         = 182m,
            Age            = 28,
            Gender         = "male",
            TargetWeight   = 78m,
            ActivityMultiplier = 1.55m,
            CurrentStreak  = 5,
            LongestStreak  = 14,
            LastStreakUpdate = new DateTime(2026, 6, 9),
        });
        await db.SaveChangesAsync();
    }

    // ── Pomiary ciała (co ~10 dni) ───────────────────────────────────────────────
    static async Task SeedMeasurementsAsync(AppDbContext db)
    {
        var data = new (DateTime D, decimal W, decimal Fat, decimal Waist, decimal Hips)[]
        {
            (new(2026,4,10), 85.0m, 19.0m, 87.0m, 98.0m),
            (new(2026,4,20), 84.5m, 18.7m, 86.5m, 97.5m),
            (new(2026,4,30), 84.0m, 18.4m, 86.0m, 97.0m),
            (new(2026,5,10), 83.5m, 18.1m, 85.5m, 96.5m),
            (new(2026,5,20), 83.0m, 17.8m, 85.0m, 96.0m),
            (new(2026,5,30), 82.2m, 17.5m, 84.5m, 95.5m),
            (new(2026,6, 9), 81.8m, 17.2m, 84.0m, 95.0m),
        };
        foreach (var (d, w, fat, waist, hips) in data)
            db.BodyMeasurements.Add(new BodyMeasurement { Id = Guid.NewGuid(), UserId = UserId, Date = d, Weight = w, BodyFatPercentage = fat, Waist = waist, Hips = hips });
        await db.SaveChangesAsync();
    }

    // ── Dziennik posiłków (prawie każdy dzień kwiecień–czerwiec) ─────────────────
    static async Task SeedMealLogsAsync(AppDbContext db)
    {
        // Dni bez wpisu (niedziele + kilka losowych)
        var skip = new HashSet<DateTime>
        {
            new(2026,4,12), new(2026,4,19), new(2026,4,26),
            new(2026,5, 3), new(2026,5,10), new(2026,5,17),
            new(2026,5,24), new(2026,5,31), new(2026,6, 7),
            new(2026,4,15), new(2026,4,22), new(2026,5, 6),
            new(2026,5,13), new(2026,5,27),
        };
        // Dni treningowe → większe porcje
        var trainingDays = new HashSet<DateTime>
        {
            new(2026,4,14), new(2026,4,16), new(2026,4,18),
            new(2026,4,21), new(2026,4,23), new(2026,4,25),
            new(2026,4,28), new(2026,4,30),
            new(2026,5, 2), new(2026,5, 5), new(2026,5, 7),
            new(2026,5, 9), new(2026,5,12), new(2026,5,14),
            new(2026,5,16), new(2026,5,19), new(2026,5,21),
            new(2026,5,23), new(2026,5,26), new(2026,5,28),
            new(2026,5,30), new(2026,6, 2), new(2026,6, 4),
            new(2026,6, 6), new(2026,6, 9),
        };

        for (var date = new DateTime(2026,4,10); date <= new DateTime(2026,6,9); date = date.AddDays(1))
        {
            if (skip.Contains(date)) continue;

            bool big = trainingDays.Contains(date);

            decimal oats    = big ? 120m : 80m;
            decimal banana  = big ? 120m : 100m;
            decimal chicken = big ? 250m : 180m;
            decimal rice    = big ? 200m : 150m;
            decimal broc    = big ? 150m : 100m;
            decimal bread   = big ? 70m  : 0m;
            decimal cottage = big ? 250m : 200m;
            decimal yogurt  = big ? 200m : 150m;

            int kcal = Kcal(oats,370) + Kcal(banana,89) + Kcal(chicken,165)
                     + Kcal(rice,130) + Kcal(broc,34) + Kcal(bread,247)
                     + Kcal(cottage,70) + Kcal(yogurt,73);

            decimal prot = M(oats,13m) + M(banana,1.1m) + M(chicken,31m)
                         + M(rice,2.7m) + M(broc,2.8m) + M(bread,9m)
                         + M(cottage,12m) + M(yogurt,5.7m);
            decimal carb = M(oats,60m) + M(banana,23m) + M(rice,28m)
                         + M(broc,6.6m) + M(bread,46m) + M(cottage,3m) + M(yogurt,3.6m);
            decimal fat  = M(oats,7m) + M(banana,0.3m) + M(chicken,3.6m)
                         + M(rice,0.3m) + M(broc,0.4m) + M(bread,3m)
                         + M(cottage,0.5m) + M(yogurt,3.3m);
            decimal fiber  = M(oats,10m) + M(banana,2.6m) + M(broc,2.6m) + M(bread,6m);
            decimal sugars = M(banana,12m) + M(yogurt,3.6m) + M(bread,4m);
            decimal satfat = M(oats,1.4m) + M(yogurt,2m);
            decimal sodium = M(chicken,70m) + M(bread,400m) + M(cottage,30m) + M(yogurt,36m);
            decimal calc   = M(oats,54m) + M(yogurt,110m) + M(cottage,83m);
            decimal iron   = M(oats,4m) + M(broc,0.7m);

            var logId = Guid.NewGuid();
            var log = new MealLog
            {
                Id = logId, UserId = UserId, Date = date,
                TotalCalories = kcal,
                TotalProtein  = Round(prot),
                TotalCarbs    = Round(carb),
                TotalFats     = Round(fat),
                TotalFiber    = Round(fiber),
                TotalSugars   = Round(sugars),
                TotalSaturatedFat = Round(satfat),
                TotalSodium   = Round(sodium),
                TotalCalcium  = Round(calc),
                TotalIron     = Round(iron),
            };

            log.Items.Add(new MealLogItem { Id = Guid.NewGuid(), MealLogId = logId, FoodProductId = FOats,     Grams = oats    });
            log.Items.Add(new MealLogItem { Id = Guid.NewGuid(), MealLogId = logId, FoodProductId = FBanana,   Grams = banana  });
            log.Items.Add(new MealLogItem { Id = Guid.NewGuid(), MealLogId = logId, FoodProductId = FChicken,  Grams = chicken });
            log.Items.Add(new MealLogItem { Id = Guid.NewGuid(), MealLogId = logId, FoodProductId = FRice,     Grams = rice    });
            log.Items.Add(new MealLogItem { Id = Guid.NewGuid(), MealLogId = logId, FoodProductId = FBroccoli, Grams = broc    });
            log.Items.Add(new MealLogItem { Id = Guid.NewGuid(), MealLogId = logId, FoodProductId = FCottage,  Grams = cottage });
            log.Items.Add(new MealLogItem { Id = Guid.NewGuid(), MealLogId = logId, FoodProductId = FYogurt,   Grams = yogurt  });
            if (bread > 0)
                log.Items.Add(new MealLogItem { Id = Guid.NewGuid(), MealLogId = logId, FoodProductId = FBread, Grams = bread });

            db.MealLogs.Add(log);
        }
        await db.SaveChangesAsync();
    }

    // ── Woda (każdy dzień, zmienne ilości) ───────────────────────────────────────
    static async Task SeedWaterLogsAsync(AppDbContext db)
    {
        int[] ml = { 1800, 2100, 2400, 2600, 1600, 2800, 2000, 2300, 1750, 2500, 2200, 1900, 2700, 2050 };
        int i = 0;
        for (var d = new DateTime(2026,4,10); d <= new DateTime(2026,6,9); d = d.AddDays(1))
        {
            db.WaterLogs.Add(new WaterLog(UserId, d, ml[i++ % ml.Length]));
        }
        await db.SaveChangesAsync();
    }

    // ── Treningi Push/Pull/Legs z progresją (2 miesiące) ────────────────────────
    static async Task SeedWorkoutsAsync(AppDbContext db)
    {
        // PUSH — poniedziałki
        var pushDates   = Dates(2026,4,14, 2026,4,21, 2026,4,28, 2026,5,5,  2026,5,12,
                                2026,5,19, 2026,5,26, 2026,6,2,  2026,6,9);
        decimal[] bench = { 75m, 77.5m, 80m, 82.5m, 85m, 87.5m, 90m, 92.5m, 95m };
        decimal[] ohp   = { 47.5m, 50m, 52.5m, 55m, 57.5m, 60m, 62.5m, 65m, 67.5m };
        decimal[] tri   = { 25m, 27.5m, 27.5m, 30m, 30m, 32.5m, 35m, 35m, 37.5m };
        for (int i = 0; i < pushDates.Length; i++)
        {
            var s = new WorkoutSession(UserId, pushDates[i], "Push", 65);
            Sets(s, ExBench, bench[i], 4, 8, 8, 7, 6);
            Sets(s, ExOhp,   ohp[i],   3, 10, 9, 8);
            Sets(s, ExTricep,tri[i],   3, 12, 10, 10);
            db.WorkoutSessions.Add(s);
        }

        // PULL — środy
        var pullDates    = Dates(2026,4,16, 2026,4,23, 2026,4,30, 2026,5,7,  2026,5,14,
                                 2026,5,21, 2026,5,28, 2026,6,4);
        decimal[] dead   = { 110m, 115m, 120m, 125m, 130m, 135m, 140m, 145m };
        decimal[] rows   = { 65m, 67.5m, 70m, 72.5m, 75m, 80m, 82.5m, 85m };
        decimal[] bicep  = { 30m, 30m, 32.5m, 35m, 35m, 37.5m, 40m, 42.5m };
        for (int i = 0; i < pullDates.Length; i++)
        {
            var s = new WorkoutSession(UserId, pullDates[i], "Pull", 70);
            Sets(s, ExDeadlift, dead[i],  3, 5, 5, 4);
            Sets(s, ExRows,     rows[i],  4, 10, 10, 9, 8);
            Sets(s, ExPullups,  0m,       3, 8, 7, 6);
            Sets(s, ExBicep,    bicep[i], 3, 12, 10, 10);
            db.WorkoutSessions.Add(s);
        }

        // LEGS — piątki
        var legDates    = Dates(2026,4,18, 2026,4,25, 2026,5,2,  2026,5,9,  2026,5,16,
                                2026,5,23, 2026,5,30, 2026,6,6);
        decimal[] squat = { 90m, 92.5m, 95m, 97.5m, 100m, 102.5m, 107.5m, 112.5m };
        decimal[] lunge = { 18m, 18m, 20m, 20m, 22.5m, 22.5m, 25m, 27.5m };
        decimal[] legex = { 60m, 62.5m, 65m, 65m, 67.5m, 70m, 72.5m, 75m };
        for (int i = 0; i < legDates.Length; i++)
        {
            var s = new WorkoutSession(UserId, legDates[i], "Legs", 75);
            Sets(s, ExSquat,  squat[i], 4, 8, 8, 7, 6);
            Sets(s, ExLunges, lunge[i], 3, 12, 12, 10);
            Sets(s, ExLegExt, legex[i], 3, 12, 12, 10);
            db.WorkoutSessions.Add(s);
        }

        await db.SaveChangesAsync();
    }

    // ── Helpers ──────────────────────────────────────────────────────────────────
    static int     Kcal(decimal g, int p100) => (int)(g * p100 / 100m);
    static decimal M(decimal g, decimal p100) => g * p100 / 100m;
    static decimal Round(decimal v) => Math.Round(v, 1);

    static void Sets(WorkoutSession s, Guid exId, decimal weight, int count, params int[] reps)
    {
        for (int i = 0; i < count; i++)
            s.Sets.Add(new SetEntry(exId, i + 1, weight, reps[Math.Min(i, reps.Length - 1)]));
    }

    static DateTime[] Dates(params int[] ymd)
    {
        var result = new DateTime[ymd.Length / 3];
        for (int i = 0; i < result.Length; i++)
            result[i] = new DateTime(ymd[i * 3], ymd[i * 3 + 1], ymd[i * 3 + 2]);
        return result;
    }
}
