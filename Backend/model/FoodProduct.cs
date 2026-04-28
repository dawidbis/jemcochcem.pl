namespace FitApp.Domain.Entities;

public class FoodProduct
{
    public string Barcode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public MacroNutrients MacrosPer100g { get; set; } = MacroNutrients.Zero();
}