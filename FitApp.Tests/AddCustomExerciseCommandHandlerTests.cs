using System;
using System.Threading;
using System.Threading.Tasks;
using FitApp.Application.Features.Workouts.Commands;
using FitApp.Domain.Entities;
using FitApp.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

public class AddCustomExerciseCommandHandlerTests
{
    private readonly Mock<IWorkoutRepository> _repoMock = new();
    private readonly AddCustomExerciseCommandHandler _handler;

    public AddCustomExerciseCommandHandlerTests()
        => _handler = new AddCustomExerciseCommandHandler(_repoMock.Object);

    [Fact]
    public async Task Handle_ShouldPersistCustomExercise_AndReturnId()
    {
        var userId = Guid.NewGuid();
        var command = new AddCustomExerciseCommand(userId, "  Martwy ciąg  ", "Plecy");

        Exercise? captured = null;
        _repoMock
            .Setup(r => r.AddExerciseAsync(It.IsAny<Exercise>()))
            .Callback<Exercise>(e => captured = e)
            .Returns(Task.CompletedTask);

        var id = await _handler.Handle(command, CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.Name.Should().Be("Martwy ciąg");   // trim zastosowany
        captured.MuscleGroup.Should().Be("Plecy");
        captured.IsCustom.Should().BeTrue();
        captured.UserId.Should().Be(userId);
        id.Should().Be(captured.Id);

        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldSaveChanges_ExactlyOnce()
    {
        _repoMock.Setup(r => r.AddExerciseAsync(It.IsAny<Exercise>())).Returns(Task.CompletedTask);

        await _handler.Handle(new AddCustomExerciseCommand(Guid.NewGuid(), "Squat", "Nogi"), CancellationToken.None);

        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
