namespace FitApp.Domain.Services;

using FitApp.Domain.Entities;
using FitApp.Domain.Interfaces;
using System;

public class MealLogDomainService : IMealLogDomainService
{
    private readonly INutritionCalculationService _nutritionService;
    
    // Zauważ, że wstrzykujemy interfejs, a nie konkretną klasę!
    public MealLogDomainService(INutritionCalculationService nutritionService) => 
        _nutritionService = nutritionService;

    public void AddItemToLog(MealLog log, MealLogItem item) 
    {
        log.Items.Add(item);
        RecalculateLogTotals(log);
    }

   public void RecalculateLogTotals(MealLog log)
   {
       log.TotalCalories = 0;
       log.TotalProtein = 0;
       log.TotalCarbs = 0;
       log.TotalFats = 0;

       log.TotalFiber = 0;
       log.TotalSugars = 0;
       log.TotalSaturatedFat = 0;
       log.TotalSodium = 0;
       log.TotalCalcium = 0;
       log.TotalIron = 0;

       foreach (var item in log.Items)
       {
           var product = item.FoodProduct;
           ArgumentNullException.ThrowIfNull(product);

           log.TotalCalories += _nutritionService.CalculateItemCalories(item.Grams, product.CaloriesPer100g);

           // UŻYWAMY NASZEJ METODY ZAMIAST LICZYĆ RĘCZNIE!
           var macros = _nutritionService.CalculateItemMacros(item.Grams, product.ProteinPer100g, product.CarbsPer100g, product.FatsPer100g);

           log.TotalProtein += macros.Protein;
           log.TotalCarbs += macros.Carbs;
           log.TotalFats += macros.Fats;

           // Analogicznie liczymy mikroskładniki
           var micros = _nutritionService.CalculateItemMicros(
               item.Grams,
               product.FiberPer100g,
               product.SugarsPer100g,
               product.SaturatedFatPer100g,
               product.SodiumPer100g,
               product.CalciumPer100g,
               product.IronPer100g);

           log.TotalFiber += micros.Fiber;
           log.TotalSugars += micros.Sugars;
           log.TotalSaturatedFat += micros.SaturatedFat;
           log.TotalSodium += micros.Sodium;
           log.TotalCalcium += micros.Calcium;
           log.TotalIron += micros.Iron;
       }
   }
}