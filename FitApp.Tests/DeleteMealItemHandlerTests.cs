using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FitApp.Application.Features.Diet;
using FitApp.Domain.Entities;
using FitApp.Domain.Interfaces;
using FitApp.Infrastructure.Interfaces;
using Moq;
using FluentAssertions;
using Xunit;

public class DeleteMealItemHandlerTests
{
    private readonly Mock<IMealLogRepository> _mealRepoMock = new();
    private readonly Mock<IMealLogDomainService> _domainServiceMock = new();
    private readonly DeleteMealItemHandler _handler;

    public DeleteMealItemHandlerTests()
        => _handler = new DeleteMealItemHandler(_mealRepoMock.Object, _domainServiceMock.Object);

    private MealLog LogWithItem(Guid userId, DateTime date, Guid itemId)
    {
        var log = new MealLog { Id = Guid.NewGuid(), UserId = userId, Date = date };
        log.Items.Add(new MealLogItem { Id = itemId, MealLogId = log.Id });
        return log;
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenLogNotFound()
    {
        _mealRepoMock.Setup(r => r.GetByDateAsync(It.IsAny<Guid>(), It.IsAny<DateTime>()))
            .ReturnsAsync((MealLog?)null);

        var act = () => _handler.Handle(
            new DeleteMealItemCommand(Guid.NewGuid(), DateTime.Today, Guid.NewGuid()),
            CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Dziennik nie istnieje*");
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenItemNotInLog()
    {
        var userId = Guid.NewGuid();
        var log = LogWithItem(userId, DateTime.Today, Guid.NewGuid());
        _mealRepoMock.Setup(r => r.GetByDateAsync(userId, DateTime.Today)).ReturnsAsync(log);

        var act = () => _handler.Handle(
            new DeleteMealItemCommand(userId, DateTime.Today, Guid.NewGuid()),
            CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Nie znaleziono posiłku*");
    }

    [Fact]
    public async Task Handle_ShouldRemoveItem_AndRecalculateTotals_AndUpdateLog()
    {
        var userId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var log = LogWithItem(userId, DateTime.Today, itemId);
        _mealRepoMock.Setup(r => r.GetByDateAsync(userId, DateTime.Today)).ReturnsAsync(log);

        await _handler.Handle(new DeleteMealItemCommand(userId, DateTime.Today, itemId), CancellationToken.None);

        log.Items.Should().BeEmpty();
        _domainServiceMock.Verify(s => s.RecalculateLogTotals(log), Times.Once);
        _mealRepoMock.Verify(r => r.UpdateAsync(log), Times.Once);
    }
}
