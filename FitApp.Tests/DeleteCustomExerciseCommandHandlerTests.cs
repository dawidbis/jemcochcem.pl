using System;
using System.Threading;
using System.Threading.Tasks;
using FitApp.Application.Features.Workouts.Commands;
using FitApp.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

public class DeleteCustomExerciseCommandHandlerTests
{
    private readonly Mock<IWorkoutRepository> _repoMock = new();
    private readonly DeleteCustomExerciseCommandHandler _handler;

    public DeleteCustomExerciseCommandHandlerTests()
        => _handler = new DeleteCustomExerciseCommandHandler(_repoMock.Object);

    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenExerciseDeleted()
    {
        var userId = Guid.NewGuid();
        var exerciseId = Guid.NewGuid();
        _repoMock.Setup(r => r.DeleteExerciseAsync(userId, exerciseId)).ReturnsAsync(true);

        var result = await _handler.Handle(new DeleteCustomExerciseCommand(userId, exerciseId), CancellationToken.None);

        result.Should().BeTrue();
        _repoMock.Verify(r => r.DeleteExerciseAsync(userId, exerciseId), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenExerciseNotFound()
    {
        _repoMock.Setup(r => r.DeleteExerciseAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(false);

        var result = await _handler.Handle(new DeleteCustomExerciseCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        result.Should().BeFalse();
    }
}
