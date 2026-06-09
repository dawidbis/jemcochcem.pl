using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FitApp.Domain.Entities;
using FitApp.Domain.Services;
using FitApp.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

public class GetWorkoutHistoryQueryHandlerTests
{
    private readonly Mock<IWorkoutRepository> _repoMock;
    private readonly GetWorkoutHistoryQueryHandler _handler;

    public GetWorkoutHistoryQueryHandlerTests()
    {
        _repoMock = new Mock<IWorkoutRepository>();
        // Używamy prawdziwego serwisu kalkulacji – chcemy sprawdzić też policzony tonaż
        _handler = new GetWorkoutHistoryQueryHandler(_repoMock.Object, new WorkoutCalculationService());
    }

    [Fact]
    public async Task Handle_ShouldMapSessionsToDto_AndCalculateVolume()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bench = new Exercise("Wyciskanie sztangi leżąc", "Klatka");

        var session = new WorkoutSession(userId, DateTime.Today, "push", 55);
        session.Sets.Add(new SetEntry(bench.Id, 1, 80m, 8) { Exercise = bench });
        session.Sets.Add(new SetEntry(bench.Id, 2, 80m, 6) { Exercise = bench });

        _repoMock
            .Setup(r => r.GetSessionsAsync(userId, null, null))
            .ReturnsAsync(new List<WorkoutSession> { session });

        // Act
        var result = await _handler.Handle(new GetWorkoutHistoryQuery(userId, null, null), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        var dto = result.First();
        dto.TotalSets.Should().Be(2);
        dto.TotalVolume.Should().Be(80m * 8 + 80m * 6); // 1120
        dto.Notes.Should().Be("push");
        dto.DurationMinutes.Should().Be(55);
        dto.Sets.Should().HaveCount(2);
        dto.Sets.First().ExerciseName.Should().Be("Wyciskanie sztangi leżąc");
        dto.Sets.First().MuscleGroup.Should().Be("Klatka");
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoSessions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _repoMock
            .Setup(r => r.GetSessionsAsync(userId, null, null))
            .ReturnsAsync(new List<WorkoutSession>());

        // Act
        var result = await _handler.Handle(new GetWorkoutHistoryQuery(userId, null, null), CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldFallBackName_WhenExerciseMissing()
    {
        var userId = Guid.NewGuid();
        var session = new WorkoutSession(userId, DateTime.Today);
        session.Sets.Add(new SetEntry(Guid.NewGuid(), 1, 50m, 10)); // Exercise == null

        _repoMock
            .Setup(r => r.GetSessionsAsync(userId, null, null))
            .ReturnsAsync(new List<WorkoutSession> { session });

        // Act
        var result = await _handler.Handle(new GetWorkoutHistoryQuery(userId, null, null), CancellationToken.None);

        // Assert
        result.First().Sets.First().ExerciseName.Should().Be("Nieznane ćwiczenie");
    }
}