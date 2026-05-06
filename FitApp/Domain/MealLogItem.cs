namespace FitApp.Domain.Entities;
public class MealLogItem 
{
    public Guid Id { get; set; }
    public Guid MealLogId { get; set; }
    public Guid FoodProductId { get; set; }
    public decimal Grams { get; set; }

    public FoodProduct? FoodProduct { get; set; } 
}