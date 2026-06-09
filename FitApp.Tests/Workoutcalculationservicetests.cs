namespace FitApp.UnitTests.Domain;

using System;
using System.Collections.Generic;
using FitApp.Domain.Entities;
using FitApp.Domain.Services;
using FluentAssertions;
using Xunit;

public class WorkoutCalculationServiceTests
{
    private readonly WorkoutCalculationService _service = new();

    [Fact]
    public void CalculateSessionVolume_ShouldSumWeightTimesReps()
    {
        // Arrange
        var sets = new List<SetEntry>
        {
            new(Guid.NewGuid(), 1, 80m, 8),   // 640
            new(Guid.NewGuid(), 2, 100m, 5),  // 500
        };

        // Act
        var result = _service.CalculateSessionVolume(sets);

        // Assert
        result.Should().Be(1140m); // 640 + 500
    }

    [Fact]
    public void CalculateSessionVolume_ShouldReturnZero_ForEmptyOrNull()
    {
        _service.CalculateSessionVolume(new List<SetEntry>()).Should().Be(0m);
        _service.CalculateSessionVolume(null!).Should().Be(0m);
    }

    [Theory]
    [InlineData(100, 1, 100.00)]    // 1 powtórzenie = czysty ciężar
    [InlineData(100, 5, 116.67)]    // Epley: 100 * (1 + 5/30)
    [InlineData(80, 10, 106.67)]    // Epley: 80 * (1 + 10/30)
    public void EstimateOneRepMax_ShouldFollowEpleyFormula(double weight, int reps, double expected)
    {
        // Act
        var result = _service.EstimateOneRepMax((decimal)weight, reps);

        // Assert
        result.Should().BeApproximately((decimal)expected, 0.01m);
    }

    [Fact]
    public void EstimateOneRepMax_ShouldReturnZero_WhenRepsInvalid()
    {
        _service.EstimateOneRepMax(100m, 0).Should().Be(0m);
        _service.EstimateOneRepMax(100m, -3).Should().Be(0m);
    }
}