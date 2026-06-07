namespace FitApp.Application.DTOs;

public class DiaryDto
{
    public DateTime Date { get; set; }
    public int TotalCalories { get; set; }
    public decimal TotalProtein { get; set; }
    public decimal TotalCarbs { get; set; }
    public decimal TotalFats { get; set; }

    // Zsumowane mikroskładniki dnia
    public decimal TotalFiber { get; set; }
    public decimal TotalSugars { get; set; }
    public decimal TotalSaturatedFat { get; set; }
    public decimal TotalSodium { get; set; }
    public decimal TotalCalcium { get; set; }
    public decimal TotalIron { get; set; }

    public List<MealLogItemDto> Items { get; set; } = new();
}