using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FitApp.Application.DTOs;
using FitApp.Application.Features.Diet;
using FitApp.Domain.Entities;
using FitApp.Domain.Interfaces;
using FitApp.Domain.Services;
using FitApp.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

public class GetDailyDiaryHandlerTests
{
    private readonly Mock<IMealLogRepository> _mealRepoMock = new();
    private readonly GetDailyDiaryHandler _handler;

    public GetDailyDiaryHandlerTests()
    {
        _handler = new GetDailyDiaryHandler(
            _mealRepoMock.Object,
            new NutritionCalculationService());
    }

    private FoodProduct MakeFood(string name, int kcal, decimal protein, decimal carbs, decimal fats)
        => new() { Id = Guid.NewGuid(), Name = name, CaloriesPer100g = kcal, ProteinPer100g = protein, CarbsPer100g = carbs, FatsPer100g = fats };

    [Fact]
    public async Task Handle_ShouldReturnEmptyDiary_WhenNoLogExists()
    {
        var userId = Guid.NewGuid();
        _mealRepoMock.Setup(r => r.GetByDateAsync(userId, DateTime.Today)).ReturnsAsync((MealLog?)null);

        var result = await _handler.Handle(new GetDailyDiaryQuery { UserId = userId, Date = DateTime.Today }, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().BeNullOrEmpty();
        result.TotalCalories.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldMapItems_WithCalculatedCaloriesAndMacros()
    {
        var userId = Guid.NewGuid();
        var food = MakeFood("Kurczak", 165, protein: 31m, carbs: 0m, fats: 3.6m);
        var log = new MealLog { Id = Guid.NewGuid(), UserId = userId, Date = DateTime.Today, TotalCalories = 330 };
        log.Items.Add(new MealLogItem { Id = Guid.NewGuid(), MealLogId = log.Id, Grams = 200m, FoodProduct = food, FoodProductId = food.Id });

        _mealRepoMock.Setup(r => r.GetByDateAsync(userId, DateTime.Today)).ReturnsAsync(log);

        var result = await _handler.Handle(new GetDailyDiaryQuery { UserId = userId, Date = DateTime.Today }, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        var item = result.Items[0];
        item.FoodName.Should().Be("Kurczak");
        item.Grams.Should().Be(200m);
        item.Calories.Should().Be(330);
        item.Macros.Protein.Should().BeApproximately(62m, 0.01m);
    }

    [Fact]
    public async Task Handle_ShouldQueryDb_ForEveryRequest()
    {
        var userId = Guid.NewGuid();
        var log = new MealLog { Id = Guid.NewGuid(), UserId = userId, Date = DateTime.Today };
        _mealRepoMock.Setup(r => r.GetByDateAsync(userId, DateTime.Today)).ReturnsAsync(log);

        await _handler.Handle(new GetDailyDiaryQuery { UserId = userId, Date = DateTime.Today }, CancellationToken.None);
        await _handler.Handle(new GetDailyDiaryQuery { UserId = userId, Date = DateTime.Today }, CancellationToken.None);

        _mealRepoMock.Verify(r => r.GetByDateAsync(userId, DateTime.Today), Times.Exactly(2));
    }
}
