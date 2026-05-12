namespace FitApp.Domain.Services;

using FitApp.Domain.Interfaces;
using FitApp.Domain.ValueObjects;
using System;

public class NutritionCalculationService : INutritionCalculationService
{
    // 1. Istniejąca metoda do posiłków (zostawiamy bez zmian!)
    public int CalculateItemCalories(decimal grams, int caloriesPer100g) => 
        (int)(grams * caloriesPer100g / 100m);

    // 2. Istniejąca metoda do posiłków (zostawiamy bez zmian!)
    public MacroNutrients CalculateItemMacros(decimal grams, decimal proteinPer100g, decimal carbsPer100g, decimal fatsPer100g) =>
        new MacroNutrients(
            grams * proteinPer100g / 100m,
            grams * carbsPer100g / 100m,
            grams * fatsPer100g / 100m
        );

    // 3. NOWA METODA: Do liczenia dziennego zapotrzebowania (celu) dla użytkownika
    public MacroNutrients CalculateDailyMacroGoals(int tdee, decimal bodyWeight)
    {
        // Białko: 2g na kg masy ciała (1g = 4 kcal)
        decimal proteinGrams = bodyWeight * 2.0m;
        
        // Tłuszcze: 25% całkowitego TDEE (1g = 9 kcal)
        decimal fatGrams = (tdee * 0.25m) / 9.0m;
        
        // Węglowodany: reszta kalorii (1g = 4 kcal)
        decimal carbsGrams = (tdee - (proteinGrams * 4.0m) - (fatGrams * 9.0m)) / 4.0m;

        // Zabezpieczenie przed wartością ujemną przy ekstremalnych kalorycznościach
        if (carbsGrams < 0) carbsGrams = 0;

        // Zakładam, że konstruktor MacroNutrients ma kolejność: (Białko, Węglowodany, Tłuszcze) 
        // - patrząc na Twojego returna w CalculateItemMacros
        return new MacroNutrients(proteinGrams, carbsGrams, fatGrams);
    }
}