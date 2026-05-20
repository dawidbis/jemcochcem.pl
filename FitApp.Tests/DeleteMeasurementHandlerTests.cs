namespace FitApp.UnitTests.Application.Features.Measurements;

using FitApp.Application.Features.Measurements;
using FitApp.Domain.Entities;
using FitApp.Infrastructure.Interfaces;
using Moq;
using Xunit;

public class DeleteMeasurementHandlerTests
{
    private readonly Mock<IBodyMeasurementRepository> _repositoryMock;
    private readonly DeleteMeasurementHandler _handler;

    public DeleteMeasurementHandlerTests()
    {
        _repositoryMock = new Mock<IBodyMeasurementRepository>();
        _handler = new DeleteMeasurementHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingMeasurement_ReturnsTrue()
    {
        // Arrange
        var measurementId = Guid.NewGuid();
        var existingMeasurement = new BodyMeasurement
        {
            Id = measurementId,
            UserId = Guid.NewGuid(),
            Weight = 75m,
            Date = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(measurementId))
            .ReturnsAsync(existingMeasurement);

        var command = new DeleteMeasurementCommand(measurementId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ExistingMeasurement_CallsRemoveAsyncOnce()
    {
        // Arrange
        var measurementId = Guid.NewGuid();
        var existingMeasurement = new BodyMeasurement
        {
            Id = measurementId,
            UserId = Guid.NewGuid(),
            Weight = 75m,
            Date = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(measurementId))
            .ReturnsAsync(existingMeasurement);

        var command = new DeleteMeasurementCommand(measurementId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetByIdAsync(measurementId), Times.Once);
        _repositoryMock.Verify(r => r.RemoveAsync(existingMeasurement), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingMeasurement_ReturnsFalse()
    {
        // Arrange
        var measurementId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(measurementId))
            .ReturnsAsync((BodyMeasurement?)null);

        var command = new DeleteMeasurementCommand(measurementId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Handle_NonExistingMeasurement_DoesNotCallRemoveAsync()
    {
        // Arrange
        var measurementId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(measurementId))
            .ReturnsAsync((BodyMeasurement?)null);

        var command = new DeleteMeasurementCommand(measurementId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetByIdAsync(measurementId), Times.Once);
        _repositoryMock.Verify(r => r.RemoveAsync(It.IsAny<BodyMeasurement>()), Times.Never);
    }

    [Fact]
    public async Task Handle_EmptyGuid_CallsGetByIdAsyncWithEmptyGuid()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(Guid.Empty))
            .ReturnsAsync((BodyMeasurement?)null);

        var command = new DeleteMeasurementCommand(Guid.Empty);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        _repositoryMock.Verify(r => r.GetByIdAsync(Guid.Empty), Times.Once);
    }

    [Fact]
    public async Task Handle_GetByIdThrows_PropagatesException()
    {
        // Arrange
        var measurementId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(measurementId))
            .ThrowsAsync(new InvalidOperationException("DB error"));

        var command = new DeleteMeasurementCommand(measurementId);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));

        _repositoryMock.Verify(r => r.RemoveAsync(It.IsAny<BodyMeasurement>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RemoveAsyncThrows_PropagatesException()
    {
        // Arrange
        var measurementId = Guid.NewGuid();
        var existingMeasurement = new BodyMeasurement
        {
            Id = measurementId,
            UserId = Guid.NewGuid(),
            Weight = 75m,
            Date = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(measurementId))
            .ReturnsAsync(existingMeasurement);

        _repositoryMock
            .Setup(r => r.RemoveAsync(It.IsAny<BodyMeasurement>()))
            .ThrowsAsync(new InvalidOperationException("DB error"));

        var command = new DeleteMeasurementCommand(measurementId);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
    }
}