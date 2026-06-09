namespace FitApp.Domain.Entities;
public class User
{   public string? RefreshToken { get; set; }
public DateTime? RefreshTokenExpiry { get; set; }
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public decimal Height { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public decimal? TargetWeight { get; set; }
    public decimal ActivityMultiplier { get; set; } = 1.55m;
    public int CurrentStreak { get; set; } = 0;
    public int LongestStreak { get; set; } = 0;
    public DateTime? LastStreakUpdate { get; set; }
    public ICollection<BodyMeasurement> BodyMeasurements { get; set; } = new List<BodyMeasurement>();
    public ICollection<MealLog> MealLogs { get; set; } = new List<MealLog>();
}
