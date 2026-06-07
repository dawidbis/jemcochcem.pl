namespace FitApp.Domain.Interfaces;

using FitApp.Domain.ValueObjects;

public interface INutritionCalculationService
{
    int CalculateItemCalories(decimal grams, int caloriesPer100g);
    MacroNutrients CalculateItemMacros(decimal grams, decimal proteinPer100g, decimal carbsPer100g, decimal fatsPer100g);
    MicroNutrients CalculateItemMicros(decimal grams, decimal fiberPer100g, decimal sugarsPer100g, decimal saturatedFatPer100g, decimal sodiumPer100g, decimal calciumPer100g, decimal ironPer100g);
}