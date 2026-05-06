namespace FitApp.UnitTests.Application;

using FitApp.Application.Features.Profile;
using FitApp.Domain.Entities;
using FitApp.Domain.Interfaces;
using FitApp.Domain.Services;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class CalculateTdeeHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly TdeeCalculationService _tdeeService;
    private readonly CalculateTdeeHandler _handler;

    public CalculateTdeeHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _tdeeService = new TdeeCalculationService();
        _handler = new CalculateTdeeHandler(_userRepositoryMock.Object, _tdeeService);
    }

    [Fact]
    public async Task Handle_ValidRequest_CalculatesAndReturnsTdee()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User 
        { 
            Id = userId, 
            Weight = 80m, 
            Height = 180m, 
            Age = 30, 
            Gender = "male" 
        };
        
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(user);

        var command = new CalculateTdeeCommand { UserId = userId, ActivityLevel = 1.2m };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        // BMR (1780) * 1.2 = 2136
        result.Should().Be(2136);
        _userRepositoryMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
    }
}