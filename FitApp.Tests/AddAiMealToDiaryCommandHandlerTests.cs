using FitApp.Application.Features.Diary.AddAiMealToDiary;
using FitApp.Domain.Entities;
using FitApp.Infrastructure.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FitApp.Tests; // Użyj namespace'u zgodnego z resztą plików w tym folderze

public class AddAiMealToDiaryCommandHandlerTests
{
    private readonly Mock<IMealPlanRepository> _mealPlanRepoMock;
    private readonly Mock<IFoodRepository> _foodRepoMock;
    private readonly Mock<IMealLogRepository> _mealLogRepoMock;
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly AddAiMealToDiaryCommandHandler _handler;

    public AddAiMealToDiaryCommandHandlerTests()
    {
        _mealPlanRepoMock = new Mock<IMealPlanRepository>();
        _foodRepoMock = new Mock<IFoodRepository>();
        _mealLogRepoMock = new Mock<IMealLogRepository>();
        _cacheMock = new Mock<IDistributedCache>();

        _handler = new AddAiMealToDiaryCommandHandler(
            _mealPlanRepoMock.Object,
            _foodRepoMock.Object,
            _mealLogRepoMock.Object,
            _cacheMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateNewMealLog_WhenLogDoesNotExist()
    {
        // Arrange
        var command = new AddAiMealToDiaryCommand(
            UserId: Guid.NewGuid(),
            MealPlanItemId: Guid.NewGuid(),
            Date: DateTime.UtcNow.Date);

        var fakePlanItem = new MealPlanItem 
        { 
            Id = command.MealPlanItemId, 
            ProductName = "Oliwa AI", 
            Grams = 10, 
            Calories = 88,
            FoodProductId = Guid.NewGuid() 
        };

        var fakeFoodProduct = new FoodProduct
        {
            Id = fakePlanItem.FoodProductId.Value,
            CaloriesPer100g = 880,
            ProteinPer100g = 0,
            CarbsPer100g = 0,
            FatsPer100g = 100
        };

        _mealPlanRepoMock.Setup(x => x.GetItemByIdAsync(command.MealPlanItemId))
                         .ReturnsAsync(fakePlanItem);
                         
        _mealLogRepoMock.Setup(x => x.GetByDateAsync(command.UserId, command.Date))
                        .ReturnsAsync((MealLog)null);

        // Mockujemy pobranie produktu do przeliczenia makro
        _foodRepoMock.Setup(x => x.GetByIdAsync(fakeFoodProduct.Id))
                     .ReturnsAsync(fakeFoodProduct);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result); 
        _mealLogRepoMock.Verify(x => x.AddAsync(It.IsAny<MealLog>()), Times.Once);
        _mealLogRepoMock.Verify(x => x.AddMealLogItemAsync(It.IsAny<MealLogItem>()), Times.Once);
        _mealLogRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}