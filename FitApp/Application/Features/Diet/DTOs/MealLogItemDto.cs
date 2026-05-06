namespace FitApp.Application.DTOs;

public class MealLogItemDto
{
    public Guid Id { get; set; }
    public string FoodName { get; set; } = string.Empty;
    public decimal Grams { get; set; }
    public int Calories { get; set; }
    public MacroNutrientsDto Macros { get; set; } = null!;
}