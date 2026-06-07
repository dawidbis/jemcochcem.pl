namespace FitApp.Domain.Entities;

public class WorkoutSession
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public string? Notes { get; set; }
    public int? DurationMinutes { get; set; }

    // Serie wykonane w ramach tego treningu
    public ICollection<SetEntry> Sets { get; set; } = new List<SetEntry>();

    public WorkoutSession() { }

    public WorkoutSession(Guid userId, DateTime date, string? notes = null, int? durationMinutes = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Date = date.Date; // zapisujemy samą datę bez godzin (spójnie z WaterLog/MealLog)
        Notes = notes;
        DurationMinutes = durationMinutes;
    }
}
