namespace FitApp.Application.DTOs;

public class DiaryDto
{
    public DateTime Date { get; set; }
    public int TotalCalories { get; set; }
    public decimal TotalProtein { get; set; }
    public decimal TotalCarbs { get; set; }
    public decimal TotalFats { get; set; }
    public List<MealLogItemDto> Items { get; set; } = new();
}