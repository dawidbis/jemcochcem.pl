namespace FitApp.UnitTests.Application.Features.Users;

using FitApp.Application.Features.Users;
using FitApp.Domain.Entities;
using FitApp.Infrastructure.Interfaces;
using Moq;
using Xunit;

public class AddMeasurementHandlerTests
{
    private readonly Mock<IBodyMeasurementRepository> _repositoryMock;
    private readonly AddMeasurementHandler _handler;

    public AddMeasurementHandlerTests()
    {
        _repositoryMock = new Mock<IBodyMeasurementRepository>();
        _handler = new AddMeasurementHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsNonEmptyGuid()
    {
        // Arrange
        var command = new AddMeasurementCommand
        {
            UserId = Guid.NewGuid(),
            Weight = 75.5m,
            Date = DateTime.UtcNow
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsRepositoryAddAsyncOnce()
    {
        // Arrange
        var command = new AddMeasurementCommand
        {
            UserId = Guid.NewGuid(),
            Weight = 80m,
            Date = DateTime.UtcNow
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<BodyMeasurement>()), Times.Once);
    }

    [Fact]
    public async Task Handle_MultipleCalls_GeneratesUniqueGuids()
    {
        // Arrange
        var command = new AddMeasurementCommand
        {
            UserId = Guid.NewGuid(),
            Weight = 70m,
            Date = DateTime.UtcNow
        };

        // Act
        var result1 = await _handler.Handle(command, CancellationToken.None);
        var result2 = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(result1, result2);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_PropagatesException()
    {
        // Arrange
        var command = new AddMeasurementCommand
        {
            UserId = Guid.NewGuid(),
            Weight = 75m,
            Date = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<BodyMeasurement>()))
            .ThrowsAsync(new InvalidOperationException("DB error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(50.5)]
    [InlineData(150.75)]
    [InlineData(300)]
    public async Task Handle_VariousWeights_CompletesSuccessfully(decimal weight)
    {
        // Arrange
        var command = new AddMeasurementCommand
        {
            UserId = Guid.NewGuid(),
            Weight = weight,
            Date = DateTime.UtcNow
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<BodyMeasurement>()), Times.Once);
    }
}