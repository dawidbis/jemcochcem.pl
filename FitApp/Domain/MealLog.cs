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

    // Zsumowane mikroskładniki dnia (g dla błonnika/cukrów/tł. nasyconych, mg dla sodu/wapnia/żelaza)
    public decimal TotalFiber { get; set; }
    public decimal TotalSugars { get; set; }
    public decimal TotalSaturatedFat { get; set; }
    public decimal TotalSodium { get; set; }
    public decimal TotalCalcium { get; set; }
    public decimal TotalIron { get; set; }

    public ICollection<MealLogItem> Items { get; set; } = new List<MealLogItem>();
}