using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FitApp.Application.DTOs;
using FitApp.Application.Features.Diet;
using FitApp.Domain.Entities;
using FitApp.Domain.Interfaces;
using FitApp.Domain.Services;
using FitApp.Infrastructure.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;

public class GetDailyDiaryHandlerTests
{
    private readonly Mock<IMealLogRepository> _mealRepoMock = new();
    private readonly Mock<IDistributedCache> _cacheMock = new();
    private readonly GetDailyDiaryHandler _handler;

    public GetDailyDiaryHandlerTests()
    {
        // Cache domyślnie zwraca null (cache miss)
        _cacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        _handler = new GetDailyDiaryHandler(
            _mealRepoMock.Object,
            new NutritionCalculationService(),
            _cacheMock.Object);
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
        item.Calories.Should().Be(330);             // 200g × 165kcal / 100
        item.Macros.Protein.Should().BeApproximately(62m, 0.01m);  // 200 × 31 / 100
    }

    [Fact]
    public async Task Handle_ShouldReturnCachedResult_WhenCacheHit()
    {
        var userId = Guid.NewGuid();
        // Serializujemy tak samo jak handler — domyślny System.Text.Json (PascalCase)
        var cachedDto = new DiaryDto { Date = new DateTime(2026, 1, 1), TotalCalories = 999, Items = new() };
        var cachedBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(cachedDto));

        _cacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedBytes);

        var result = await _handler.Handle(new GetDailyDiaryQuery { UserId = userId, Date = DateTime.Today }, CancellationToken.None);

        result.TotalCalories.Should().Be(999);
        // Cache hit — repo nie powinno być odpytane
        _mealRepoMock.Verify(r => r.GetByDateAsync(It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldWriteToCache_AfterDbRead()
    {
        var userId = Guid.NewGuid();
        var log = new MealLog { Id = Guid.NewGuid(), UserId = userId, Date = DateTime.Today };
        _mealRepoMock.Setup(r => r.GetByDateAsync(userId, DateTime.Today)).ReturnsAsync(log);

        await _handler.Handle(new GetDailyDiaryQuery { UserId = userId, Date = DateTime.Today }, CancellationToken.None);

        _cacheMock.Verify(c => c.SetAsync(
            It.Is<string>(k => k.StartsWith("diary:")),
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
