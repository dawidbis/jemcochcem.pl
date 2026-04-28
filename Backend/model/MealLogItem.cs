using System;

namespace FitApp.Domain.Entities;

public class MealLogItem
{
    public Guid Id { get; set; }
    public Guid FoodProductId { get; set; }
    public double WeightGrams { get; set; }
    public MacroNutrients CalculatedMacros { get; set; } = MacroNutrients.Zero();

    public void Recalculate(MacroNutrients baseMacros)
    {
        var multiplier = WeightGrams / 100.0;
        CalculatedMacros = new MacroNutrients
        {
            Protein = baseMacros.Protein * multiplier,
            Carbs = baseMacros.Carbs * multiplier,
            Fat = baseMacros.Fat * multiplier,
            Kcal = (int)Math.Round(baseMacros.Kcal * multiplier)
        };
    }
}