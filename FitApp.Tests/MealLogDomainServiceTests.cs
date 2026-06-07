namespace FitApp.UnitTests.Domain;

using FitApp.Domain.Entities;
using FitApp.Domain.Interfaces;
using FitApp.Domain.Services;
using FitApp.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using Xunit;

public class MealLogDomainServiceTests
{
    private readonly MealLogDomainService _service;
    private readonly Mock<INutritionCalculationService> _nutritionServiceMock;

    public MealLogDomainServiceTests()
    {
        _nutritionServiceMock = new Mock<INutritionCalculationService>();
        _service = new MealLogDomainService(_nutritionServiceMock.Object);
    }

    [Fact]
    public void AddItemToLog_CalculatesTotalsCorrectly()
    {
        // Arrange
        var log = new MealLog();
        var product = new FoodProduct
        {
            CaloriesPer100g = 250, ProteinPer100g = 10m, CarbsPer100g = 20m, FatsPer100g = 15m,
            FiberPer100g = 2m, SugarsPer100g = 5m, SaturatedFatPer100g = 4m,
            SodiumPer100g = 100m, CalciumPer100g = 50m, IronPer100g = 1m
        };
        var item = new MealLogItem { Grams = 200m, FoodProduct = product };

        _nutritionServiceMock
            .Setup(s => s.CalculateItemCalories(200m, 250))
            .Returns(500);

        _nutritionServiceMock
            .Setup(s => s.CalculateItemMacros(200m, 10m, 20m, 15m))
            .Returns(new MacroNutrients(20m, 40m, 30m));

        _nutritionServiceMock
            .Setup(s => s.CalculateItemMicros(200m, 2m, 5m, 4m, 100m, 50m, 1m))
            .Returns(new MicroNutrients(4m, 10m, 8m, 200m, 100m, 2m));

        // Act
        _service.AddItemToLog(log, item);

        // Assert
        log.TotalCalories.Should().Be(500);
        log.TotalProtein.Should().Be(20m);
        log.TotalCarbs.Should().Be(40m);
        log.TotalFats.Should().Be(30m);

        log.TotalFiber.Should().Be(4m);
        log.TotalSugars.Should().Be(10m);
        log.TotalSaturatedFat.Should().Be(8m);
        log.TotalSodium.Should().Be(200m);
        log.TotalCalcium.Should().Be(100m);
        log.TotalIron.Should().Be(2m);

        // Weryfikacja czy metody zostały wywołane
        _nutritionServiceMock.Verify(s => s.CalculateItemCalories(200m, 250), Times.Once);
        _nutritionServiceMock.Verify(s => s.CalculateItemMacros(200m, 10m, 20m, 15m), Times.Once);
        _nutritionServiceMock.Verify(s => s.CalculateItemMicros(200m, 2m, 5m, 4m, 100m, 50m, 1m), Times.Once);
    }
}