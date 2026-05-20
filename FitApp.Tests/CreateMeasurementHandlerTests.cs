namespace FitApp.UnitTests.Application.Features.Diet;

using FitApp.Application.Features.Diet;
using FitApp.Domain.Entities;
using FitApp.Infrastructure.Interfaces;
using Moq;
using Xunit;

public class CreateMeasurementHandlerTests
{
    private readonly Mock<IBodyMeasurementRepository> _repositoryMock;
    private readonly CreateMeasurementHandler _handler;

    public CreateMeasurementHandlerTests()
    {
        _repositoryMock = new Mock<IBodyMeasurementRepository>();
        _handler = new CreateMeasurementHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommandWithAllFields_ReturnsNonEmptyGuid()
    {
        // Arrange
        var command = new CreateMeasurementCommand(
            UserId: Guid.NewGuid(),
            Weight: 75.5m,
            Date: DateTime.UtcNow,
            BodyFatPercentage: 18.5m,
            Waist: 80m,
            Hips: 95m,
            Notes: "After workout"
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
    }

    [Fact]
    public async Task Handle_ValidCommandWithOnlyRequiredFields_ReturnsNonEmptyGuid()
    {
        // Arrange
        var command = new CreateMeasurementCommand(
            UserId: Guid.NewGuid(),
            Weight: 75.5m,
            Date: DateTime.UtcNow
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsRepositoryAddAsyncOnce()
    {
        // Arrange
        var command = new CreateMeasurementCommand(
            UserId: Guid.NewGuid(),
            Weight: 80m,
            Date: DateTime.UtcNow
        );

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<BodyMeasurement>()), Times.Once);
    }

    [Fact]
    public async Task Handle_MultipleCalls_GeneratesUniqueGuids()
    {
        // Arrange
        var command = new CreateMeasurementCommand(
            UserId: Guid.NewGuid(),
            Weight: 70m,
            Date: DateTime.UtcNow
        );

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
        var command = new CreateMeasurementCommand(
            UserId: Guid.NewGuid(),
            Weight: 75m,
            Date: DateTime.UtcNow
        );

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<BodyMeasurement>()))
            .ThrowsAsync(new InvalidOperationException("DB error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Theory]
    [InlineData(null, null, null, null)]
    [InlineData(15.0, null, null, "Notes only fat")]
    [InlineData(null, 75.0, 90.0, null)]
    [InlineData(20.5, 80.0, 95.0, "Full data")]
    public async Task Handle_VariousOptionalFieldCombinations_CompletesSuccessfully(
        double? bodyFat, double? waist, double? hips, string? notes)
    {
        // Arrange
        var command = new CreateMeasurementCommand(
            UserId: Guid.NewGuid(),
            Weight: 75m,
            Date: DateTime.UtcNow,
            BodyFatPercentage: (decimal?)bodyFat,
            Waist: (decimal?)waist,
            Hips: (decimal?)hips,
            Notes: notes
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<BodyMeasurement>()), Times.Once);
    }
}