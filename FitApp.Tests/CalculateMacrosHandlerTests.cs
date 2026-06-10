using System;
using System.Threading;
using System.Threading.Tasks;
using FitApp.Application.Features.Users;
using FitApp.Domain.Entities;
using FitApp.Domain.Services;
using FitApp.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

public class CalculateMacrosHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly CalculateMacrosHandler _handler;

    public CalculateMacrosHandlerTests()
    {
        _handler = new CalculateMacrosHandler(
            _userRepoMock.Object,
            new TdeeCalculationService(),
            new NutritionCalculationService());
    }

    private User MakeUser(decimal weight, decimal height, int age, string gender, decimal activity)
        => new() { Id = Guid.NewGuid(), Weight = weight, Height = height, Age = age, Gender = gender, ActivityMultiplier = activity };

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserNotFound()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        var act = () => _handler.Handle(new CalculateMacrosCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*użytkownika*");
    }

    [Fact]
    public async Task Handle_ShouldReturnPositiveTdee_ForTypicalMaleUser()
    {
        var user = MakeUser(weight: 80, height: 180, age: 30, gender: "male", activity: 1.55m);
        _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

        var result = await _handler.Handle(new CalculateMacrosCommand(user.Id), CancellationToken.None);

        result.Tdee.Should().BeGreaterThan(2000).And.BeLessThan(4000);
    }

    [Fact]
    public async Task Handle_ShouldSetProtein_To2gPerKg()
    {
        var user = MakeUser(weight: 80, height: 180, age: 30, gender: "male", activity: 1.55m);
        _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

        var result = await _handler.Handle(new CalculateMacrosCommand(user.Id), CancellationToken.None);

        // 80kg × 2g = 160g białka
        result.Protein.Should().Be(160);
    }

    [Fact]
    public async Task Handle_ShouldSetFat_To25PercentOfTdee()
    {
        var user = MakeUser(weight: 80, height: 180, age: 30, gender: "male", activity: 1.55m);
        _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

        var result = await _handler.Handle(new CalculateMacrosCommand(user.Id), CancellationToken.None);

        // tłuszcze = (tdee * 0.25) / 9 zaokrąglone do int
        int expectedFat = (int)((result.Tdee * 0.25m) / 9m);
        result.Fat.Should().Be(expectedFat);
    }

    [Fact]
    public async Task Handle_ShouldReturnNonNegativeCarbs_ForHighProteinUser()
    {
        // Bardzo mała kobieta z niską kalorycznością — węglowodany mogłyby wyjść ujemne
        var user = MakeUser(weight: 40, height: 155, age: 25, gender: "female", activity: 1.2m);
        _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

        var result = await _handler.Handle(new CalculateMacrosCommand(user.Id), CancellationToken.None);

        result.Carbs.Should().BeGreaterThanOrEqualTo(0);
    }

    [Theory]
    [InlineData("male",   1.2)]
    [InlineData("female", 1.55)]
    [InlineData("male",   1.9)]
    public async Task Handle_ShouldReturnPositiveValuesForAllActivityLevels(string gender, double activity)
    {
        var user = MakeUser(weight: 70, height: 170, age: 28, gender: gender, activity: (decimal)activity);
        _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

        var result = await _handler.Handle(new CalculateMacrosCommand(user.Id), CancellationToken.None);

        result.Tdee.Should().BePositive();
        result.Protein.Should().BePositive();
        result.Fat.Should().BePositive();
    }
}
