using FitApp.Domain.Entities;
using FitApp.Domain.Services;
using FitApp.Domain.ValueObjects;
using System;
using Xunit;

namespace FitApp.Tests;

public class BllTests
{
    [Fact]
    public void NutritionCalculationService_Should_Calculate_Calories_Correctly()
    {
        // Arrange
        var service = new NutritionCalculationService();
        decimal grams = 150m;
        int caloriesPer100g = 200; // np. 200 kcal w 100g

        // Act
        var result = service.CalculateItemCalories(grams, caloriesPer100g);

        // Assert
        Assert.Equal(300, result); // 1.5 * 200 = 300
    }

    [Fact]
    public void MealLogDomainService_Should_Recalculate_Totals_Correctly()
    {
        // Arrange
        var nutritionService = new NutritionCalculationService();
        var mealLogService = new MealLogDomainService(nutritionService);

        var log = new MealLog { Id = Guid.NewGuid(), Date = DateTime.UtcNow };
        var product = new FoodProduct
        {
            CaloriesPer100g = 100,
            ProteinPer100g = 10m,
            CarbsPer100g = 20m,
            FatsPer100g = 5m,
            FiberPer100g = 3m,
            SugarsPer100g = 8m,
            SaturatedFatPer100g = 2m,
            SodiumPer100g = 120m,
            CalciumPer100g = 60m,
            IronPer100g = 1.5m
        };

        var item = new MealLogItem { Grams = 200m, FoodProduct = product };

        // Act
        mealLogService.AddItemToLog(log, item);

        // Assert
        Assert.Single(log.Items);
        Assert.Equal(200, log.TotalCalories); // 200g * 100kcal/100g
        Assert.Equal(20m, log.TotalProtein);  // 200g * 10g/100g

        // Mikroskładniki (200g => mnożnik 2x)
        Assert.Equal(6m, log.TotalFiber);          // 200g * 3g/100g
        Assert.Equal(16m, log.TotalSugars);        // 200g * 8g/100g
        Assert.Equal(4m, log.TotalSaturatedFat);   // 200g * 2g/100g
        Assert.Equal(240m, log.TotalSodium);       // 200g * 120mg/100g
        Assert.Equal(120m, log.TotalCalcium);      // 200g * 60mg/100g
        Assert.Equal(3m, log.TotalIron);           // 200g * 1.5mg/100g
    }

    [Fact]
    public void NutritionCalculationService_Should_Calculate_Micros_Correctly()
    {
        // Arrange
        var service = new NutritionCalculationService();

        // Act – dla 50g produktu (mnożnik 0.5x)
        MicroNutrients result = service.CalculateItemMicros(
            grams: 50m,
            fiberPer100g: 4m,
            sugarsPer100g: 10m,
            saturatedFatPer100g: 6m,
            sodiumPer100g: 200m,
            calciumPer100g: 80m,
            ironPer100g: 2m);

        // Assert
        Assert.Equal(2m, result.Fiber);
        Assert.Equal(5m, result.Sugars);
        Assert.Equal(3m, result.SaturatedFat);
        Assert.Equal(100m, result.Sodium);
        Assert.Equal(40m, result.Calcium);
        Assert.Equal(1m, result.Iron);
    }

    [Theory]
    [InlineData("female", 18)]
    [InlineData("kobieta", 18)]
    [InlineData("male", 10)]
    [InlineData("mężczyzna", 10)]
    public void CalculateDailyMicroGoals_Should_Adjust_Iron_By_Gender(string gender, int expectedIron)
    {
        // Arrange
        var service = new NutritionCalculationService();

        // Act
        var goals = service.CalculateDailyMicroGoals(gender);

        // Assert – żelazo zależne od płci, pozostałe wartości stałe
        Assert.Equal(expectedIron, goals.Iron);
        Assert.Equal(30m, goals.Fiber);
        Assert.Equal(1000m, goals.Calcium);
    }
}