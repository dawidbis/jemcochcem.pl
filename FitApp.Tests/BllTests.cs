using FitApp.Domain.Entities;
using FitApp.Domain.Services;
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
            FatsPer100g = 5m 
        };
        
        var item = new MealLogItem { Grams = 200m, FoodProduct = product };

        // Act
        mealLogService.AddItemToLog(log, item);

        // Assert
        Assert.Single(log.Items);
        Assert.Equal(200, log.TotalCalories); // 200g * 100kcal/100g
        Assert.Equal(20m, log.TotalProtein);  // 200g * 10g/100g
    }
}