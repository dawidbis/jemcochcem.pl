using System;
using System.Threading;
using System.Threading.Tasks;
using FitApp.Application.Features.Workouts.Commands;
using FitApp.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

public class DeleteWorkoutSessionCommandHandlerTests
{
    private readonly Mock<IWorkoutRepository> _repoMock = new();
    private readonly DeleteWorkoutSessionCommandHandler _handler;

    public DeleteWorkoutSessionCommandHandlerTests()
        => _handler = new DeleteWorkoutSessionCommandHandler(_repoMock.Object);

    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenSessionDeleted()
    {
        var userId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        _repoMock.Setup(r => r.DeleteSessionAsync(userId, sessionId)).ReturnsAsync(true);

        var result = await _handler.Handle(new DeleteWorkoutSessionCommand(userId, sessionId), CancellationToken.None);

        result.Should().BeTrue();
        _repoMock.Verify(r => r.DeleteSessionAsync(userId, sessionId), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenSessionNotFound()
    {
        _repoMock.Setup(r => r.DeleteSessionAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(false);

        var result = await _handler.Handle(new DeleteWorkoutSessionCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        result.Should().BeFalse();
    }
}
