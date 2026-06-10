using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FitApp.Application.Features.Workouts.Queries;
using FitApp.Domain.Entities;
using FitApp.Domain.Services;
using FitApp.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

public class GetExerciseProgressionQueryHandlerTests
{
    private readonly Mock<IWorkoutRepository> _repoMock = new();
    private readonly GetExerciseProgressionQueryHandler _handler;

    public GetExerciseProgressionQueryHandlerTests()
        => _handler = new GetExerciseProgressionQueryHandler(_repoMock.Object, new WorkoutCalculationService());

    private WorkoutSession SessionWith(Guid userId, Guid exerciseId, DateTime date, decimal weight, int reps)
    {
        var bench = new Exercise("Wyciskanie", "Klatka") { Id = exerciseId };
        var session = new WorkoutSession(userId, date);
        session.Sets.Add(new SetEntry(exerciseId, 1, weight, reps) { Exercise = bench });
        return session;
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenNoSessions()
    {
        var userId = Guid.NewGuid();
        var exId = Guid.NewGuid();
        _repoMock.Setup(r => r.GetSessionsByExerciseAsync(userId, exId)).ReturnsAsync(new List<WorkoutSession>());

        var result = await _handler.Handle(new GetExerciseProgressionQuery(userId, exId), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldBuildHistoryOrderedByDate()
    {
        var userId = Guid.NewGuid();
        var exId = Guid.NewGuid();
        var sessions = new List<WorkoutSession>
        {
            SessionWith(userId, exId, DateTime.Today,              100m, 8),
            SessionWith(userId, exId, DateTime.Today.AddDays(-7),   80m, 6),
        };
        _repoMock.Setup(r => r.GetSessionsByExerciseAsync(userId, exId)).ReturnsAsync(sessions);

        var result = await _handler.Handle(new GetExerciseProgressionQuery(userId, exId), CancellationToken.None);

        result.Should().NotBeNull();
        result!.History.Should().HaveCount(2);
        result.History[0].Date.Should().BeBefore(result.History[1].Date);  // posortowane rosnąco
        result.History[1].MaxWeight.Should().Be(100m);
    }

    // --- Algorytm progresji ---

    [Fact]
    public async Task Suggestion_ShouldAdd5kg_WhenLastRepsGreaterOrEqual10()
    {
        var userId = Guid.NewGuid();
        var exId = Guid.NewGuid();
        _repoMock.Setup(r => r.GetSessionsByExerciseAsync(userId, exId))
            .ReturnsAsync(new List<WorkoutSession> { SessionWith(userId, exId, DateTime.Today, 80m, 10) });

        var result = await _handler.Handle(new GetExerciseProgressionQuery(userId, exId), CancellationToken.None);

        result!.Suggestion!.SuggestedWeight.Should().Be(85m);   // 80 + 5
        result.Suggestion.SuggestedReps.Should().Be(10);
    }

    [Fact]
    public async Task Suggestion_ShouldAdd2_5kg_WhenLastRepsInRange5to9()
    {
        var userId = Guid.NewGuid();
        var exId = Guid.NewGuid();
        _repoMock.Setup(r => r.GetSessionsByExerciseAsync(userId, exId))
            .ReturnsAsync(new List<WorkoutSession> { SessionWith(userId, exId, DateTime.Today, 100m, 7) });

        var result = await _handler.Handle(new GetExerciseProgressionQuery(userId, exId), CancellationToken.None);

        result!.Suggestion!.SuggestedWeight.Should().Be(102.5m); // 100 + 2.5
        result.Suggestion.SuggestedReps.Should().Be(7);
    }

    [Fact]
    public async Task Suggestion_ShouldKeepWeightAndAddRep_WhenLastRepsBelow5()
    {
        var userId = Guid.NewGuid();
        var exId = Guid.NewGuid();
        _repoMock.Setup(r => r.GetSessionsByExerciseAsync(userId, exId))
            .ReturnsAsync(new List<WorkoutSession> { SessionWith(userId, exId, DateTime.Today, 120m, 3) });

        var result = await _handler.Handle(new GetExerciseProgressionQuery(userId, exId), CancellationToken.None);

        result!.Suggestion!.SuggestedWeight.Should().Be(120m);  // bez zmiany
        result.Suggestion.SuggestedReps.Should().Be(4);         // +1 powtórzenie
    }

    [Fact]
    public async Task Handle_ShouldCalculate1RM_UsingEpleyFormula()
    {
        var userId = Guid.NewGuid();
        var exId = Guid.NewGuid();
        // 100kg × 5 → Epley: 100 * (1 + 5/30) ≈ 116.67
        _repoMock.Setup(r => r.GetSessionsByExerciseAsync(userId, exId))
            .ReturnsAsync(new List<WorkoutSession> { SessionWith(userId, exId, DateTime.Today, 100m, 5) });

        var result = await _handler.Handle(new GetExerciseProgressionQuery(userId, exId), CancellationToken.None);

        result!.History[0].EstOneRepMax.Should().BeApproximately(116.67m, 0.01m);
    }
}
