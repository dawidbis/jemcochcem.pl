namespace FitApp.Domain.Services;

using FitApp.Domain.Interfaces;
using FitApp.Domain.ValueObjects;

public class NutritionCalculationService : INutritionCalculationService
{
    public int CalculateItemCalories(decimal grams, int caloriesPer100g) => 
        (int)(grams * caloriesPer100g / 100m);

    public MacroNutrients CalculateItemMacros(decimal grams, decimal proteinPer100g, decimal carbsPer100g, decimal fatsPer100g) =>
        new MacroNutrients(
            grams * proteinPer100g / 100m,
            grams * carbsPer100g / 100m,
            grams * fatsPer100g / 100m
        );
}