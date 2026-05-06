namespace FitApp.Domain.Entities;
public class MealLog 
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public int TotalCalories { get; set; }
    public decimal TotalProtein { get; set; }
    public decimal TotalCarbs { get; set; }
    public decimal TotalFats { get; set; }

    public ICollection<MealLogItem> Items { get; set; } = new List<MealLogItem>();
}