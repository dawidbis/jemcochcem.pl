// 2. Implementacja testów jednostkowych dla BLL
namespace FitApp.UnitTests.Domain;

using FitApp.Domain.Services;
using FluentAssertions;
using Xunit;

public class TdeeCalculationServiceTests
{
    private readonly TdeeCalculationService _service = new();

    [Fact]
    public void CalculateBMR_Male_ReturnsCorrectValue()
    {
        // Arrange
        decimal weight = 80m;
        decimal height = 180m;
        int age = 30;
        string gender = "male";

        // Act
        var result = _service.CalculateBMR(weight, height, age, gender);

        // Assert
        result.Should().Be(1780m); // (10*80) + (6.25*180) - (5*30) + 5
    }

    [Fact]
    public void CalculateTDEE_Female_ReturnsCorrectValue()
    {
        // Arrange
        decimal weight = 60m;
        decimal height = 165m;
        int age = 25;
        string gender = "female";
        decimal activityLevel = 1.5m;

        // Act
        var result = _service.CalculateTDEE(weight, height, age, gender, activityLevel);

        // Assert
        // BMR = (10*60) + (6.25*165) - (5*25) - 161 = 1345.25
        // TDEE = 1345.25 * 1.5 = 2017.875 -> zaokrąglone do 2018
        result.Should().Be(2018); 
    }
}