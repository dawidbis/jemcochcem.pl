using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FitApp.Domain.Entities;
using FitApp.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

public class LogWorkoutSessionCommandHandlerTests
{
    private readonly Mock<IWorkoutRepository> _repoMock;
    private readonly LogWorkoutSessionCommandHandler _handler;

    public LogWorkoutSessionCommandHandlerTests()
    {
        // Atrapa repozytorium – nie dotykamy prawdziwej bazy
        _repoMock = new Mock<IWorkoutRepository>();
        _handler = new LogWorkoutSessionCommandHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldPersistSessionWithAllSets_AndReturnId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var exerciseId = Guid.NewGuid();
        var sets = new List<SetInput>
        {
            new(exerciseId, 1, 80m, 8),
            new(exerciseId, 2, 80m, 7),
        };
        var command = new LogWorkoutSessionCommand(userId, DateTime.Today, "leg day", 60, sets);

        // Przechwytujemy sesję przekazaną do repozytorium
        WorkoutSession? captured = null;
        _repoMock
            .Setup(r => r.AddSessionAsync(It.IsAny<WorkoutSession>()))
            .Callback<WorkoutSession>(s => captured = s)
            .Returns(Task.CompletedTask);

        // Act
        var resultId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        captured.Should().NotBeNull();
        captured!.UserId.Should().Be(userId);
        captured.Notes.Should().Be("leg day");
        captured.DurationMinutes.Should().Be(60);
        captured.Sets.Should().HaveCount(2);
        captured.Sets.First().Weight.Should().Be(80m);
        captured.Sets.First().Reps.Should().Be(8);

        // Handler zwraca Id zapisanej sesji
        resultId.Should().Be(captured.Id);

        // Zmiany muszą zostać zapisane dokładnie raz
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCreateEmptySession_WhenNoSetsProvided()
    {
        // Arrange
        var command = new LogWorkoutSessionCommand(Guid.NewGuid(), DateTime.Today, null, null, new List<SetInput>());

        WorkoutSession? captured = null;
        _repoMock
            .Setup(r => r.AddSessionAsync(It.IsAny<WorkoutSession>()))
            .Callback<WorkoutSession>(s => captured = s)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        captured.Should().NotBeNull();
        captured!.Sets.Should().BeEmpty();
        _repoMock.Verify(r => r.AddSessionAsync(It.IsAny<WorkoutSession>()), Times.Once);
    }
}