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

    // 2b. Mikroskładniki dla konkretnej porcji (analogicznie do makro – liczymy proporcjonalnie do gramatury)
    public MicroNutrients CalculateItemMicros(
        decimal grams,
        decimal fiberPer100g,
        decimal sugarsPer100g,
        decimal saturatedFatPer100g,
        decimal sodiumPer100g,
        decimal calciumPer100g,
        decimal ironPer100g) =>
        new MicroNutrients(
            grams * fiberPer100g / 100m,
            grams * sugarsPer100g / 100m,
            grams * saturatedFatPer100g / 100m,
            grams * sodiumPer100g / 100m,
            grams * calciumPer100g / 100m,
            grams * ironPer100g / 100m
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

    // 4. NOWA METODA: Dzienne cele dla mikroskładników (zalecane spożycie / wartości referencyjne).
    // Wartości oparte na ogólnych zaleceniach dla dorosłych (m.in. EU NRV / WHO).
    // Żelazo zależy od płci (kobiety mają wyższe zapotrzebowanie ze względu na cykl menstruacyjny).
    public MicroNutrients CalculateDailyMicroGoals(string gender)
    {
        bool isFemale = !string.IsNullOrWhiteSpace(gender) &&
                        (gender.Trim().StartsWith("f", StringComparison.OrdinalIgnoreCase) || // female / f
                         gender.Trim().StartsWith("k", StringComparison.OrdinalIgnoreCase) || // kobieta / k
                         gender.Trim().StartsWith("w", StringComparison.OrdinalIgnoreCase));   // woman / w

        decimal fiberGoal = 30m;          // g  – błonnik
        decimal sugarsGoal = 50m;         // g  – cukry (górny limit wg WHO)
        decimal saturatedFatGoal = 20m;   // g  – tłuszcze nasycone (górny limit)
        decimal sodiumGoal = 2300m;       // mg – sód (górny limit ~5g soli)
        decimal calciumGoal = 1000m;      // mg – wapń
        decimal ironGoal = isFemale ? 18m : 10m; // mg – żelazo

        return new MicroNutrients(
            fiberGoal,
            sugarsGoal,
            saturatedFatGoal,
            sodiumGoal,
            calciumGoal,
            ironGoal);
    }
}