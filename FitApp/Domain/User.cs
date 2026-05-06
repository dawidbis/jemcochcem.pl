namespace FitApp.Domain.Entities;
public class User 
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public decimal Height { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;

    public ICollection<BodyMeasurement> BodyMeasurements { get; set; } = new List<BodyMeasurement>();
    public ICollection<MealLog> MealLogs { get; set; } = new List<MealLog>();
}