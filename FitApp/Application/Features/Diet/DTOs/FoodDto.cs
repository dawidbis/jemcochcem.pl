namespace FitApp.Application.DTOs;

public class FoodDto
{
    public string Barcode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int CaloriesPer100g { get; set; }
}