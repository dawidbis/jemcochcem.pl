namespace FitApp.Application.DTOs;

public class FoodDto
{
    public Guid Id { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int CaloriesPer100g { get; set; }

    // DODANE: Kompozycja z makroskładnikami
    public MacroNutrientsDto? Macros { get; set; }
}