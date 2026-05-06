namespace FitApp.UnitTests.Domain;

using FitApp.Domain.Entities;
using FitApp.Domain.Services;
using FluentAssertions;
using Xunit;

public class MealLogDomainServiceTests
{
    private readonly MealLogDomainService _service;
    private readonly NutritionCalculationService _nutritionService;

    public MealLogDomainServiceTests()
    {
        _nutritionService = new NutritionCalculationService();
        _service = new MealLogDomainService(_nutritionService);
    }

    [Fact]
    public void AddItemToLog_CalculatesTotalsCorrectly()
    {
        // Arrange
        var log = new MealLog();
        var product = new FoodProduct { CaloriesPer100g = 250, ProteinPer100g = 10m, CarbsPer100g = 20m, FatsPer100g = 15m };
        var item = new MealLogItem { Grams = 200m, FoodProduct = product };

        // Act
        _service.AddItemToLog(log, item);

        // Assert
        log.TotalCalories.Should().Be(500); // 250 * 2
        log.TotalProtein.Should().Be(20m);  // 10 * 2
        log.TotalCarbs.Should().Be(40m);    // 20 * 2
        log.TotalFats.Should().Be(30m);     // 15 * 2
    }
}