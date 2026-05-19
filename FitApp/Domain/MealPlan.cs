namespace FitApp.Domain.Entities;

public class MealPlan
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Prompt { get; set; } = string.Empty; // To, co wpisał użytkownik
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacja do użytkownika (zakładam, że masz encję User)
    public User? User { get; set; }
    
    // Lista elementów planu
    public ICollection<MealPlanItem> Items { get; set; } = new List<MealPlanItem>();
}