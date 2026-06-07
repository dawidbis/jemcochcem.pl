using FitApp.Application.Features.GenerateMealPlan;
using FitApp.Application.Interfaces; // Dopasuj do miejsca, gdzie masz IAiService
using FitApp.Domain.Entities;
using FitApp.Infrastructure.Interfaces;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FitApp.Tests;

public class GenerateMealPlanCommandHandlerTests
{
    private readonly Mock<IAiService> _aiServiceMock;
    private readonly Mock<IMealPlanRepository> _repositoryMock;
    private readonly GenerateMealPlanCommandHandler _handler;

    public GenerateMealPlanCommandHandlerTests()
    {
        _aiServiceMock = new Mock<IAiService>();
        _repositoryMock = new Mock<IMealPlanRepository>();

        _handler = new GenerateMealPlanCommandHandler(
            _aiServiceMock.Object, 
            _repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldParseJsonAndSaveMealPlan_WhenAiReturnsValidData()
    {
        // Arrange
        var command = new GenerateMealPlanCommand(
            UserId: Guid.NewGuid(),
            Prompt: "dieta keto 2000 kcal");

        // Udajemy odpowiedź od Gemini API
        string fakeJsonResponse = @"
        [
          {
            ""MealType"": ""Śniadanie"",
            ""ProductName"": ""Jajecznica"",
            ""Grams"": 200,
            ""Calories"": 300,
            ""Proteins"": 20.0,
            ""Carbs"": 2.0,
            ""Fats"": 22.0
          }
        ]";

        _aiServiceMock.Setup(x => x.AskCoachAsync(It.IsAny<string>()))
                      .ReturnsAsync(fakeJsonResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _repositoryMock.Verify(x => x.AddAsync(It.Is<MealPlan>(p => 
            p.UserId == command.UserId && 
            p.Items.Count == 1 &&
            p.Items.First().ProductName == "Jajecznica"
        )), Times.Once);
    }
}