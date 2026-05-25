using System;
using System.Threading;
using System.Threading.Tasks;
using FitApp.Application.Features.Water.Queries;
using FitApp.Domain.Entities;
using Moq;
using Xunit;

public class GetWaterStatusQueryHandlerTests
{
    private readonly Mock<IWaterLogRepository> _waterRepoMock;
    private readonly Mock<IMealLogRepository> _mealRepoMock;
    private readonly Mock<IBodyMeasurementRepository> _measurementRepoMock;
    private readonly GetWaterStatusQueryHandler _handler;

    public GetWaterStatusQueryHandlerTests()
    {
        // Tworzymy "atrapy" (Mocks) naszych repozytoriów, aby nie dotykać prawdziwej bazy danych
        _waterRepoMock = new Mock<IWaterLogRepository>();
        _mealRepoMock = new Mock<IMealLogRepository>();
        _measurementRepoMock = new Mock<IBodyMeasurementRepository>();

        // Wstrzykujemy atrapy do naszego prawdziwego Handlera logiki biznesowej
        _handler = new GetWaterStatusQueryHandler(
            _waterRepoMock.Object, 
            _mealRepoMock.Object, 
            _measurementRepoMock.Object
        );
    }

    [Fact] // Oznaczenie, że to jest automatyczny test jednostkowy
    public async Task Handle_ShouldIncreaseTargetBy500ml_WhenProteinIsHigh()
    {
        // Arrange (Przygotowanie danych wejściowych)
        var userId = Guid.NewGuid();
        var date = DateTime.Today;
        var query = new GetWaterStatusQuery(userId, date);

        // Symulujemy, że użytkownik waży 70kg (cel bazowy: 70 * 35 = 2450ml)
        var fakeMeasurement = new BodyMeasurement { Weight = 70m };
        _measurementRepoMock
            .Setup(r => r.GetLatestAsync(userId))
            .ReturnsAsync(fakeMeasurement);

        // KLUCZOWE: Symulujemy, że użytkownik zjadł 150g białka (próg > 140g przekroczony!)
        var fakeMealLog = new MealLog { TotalProtein = 150 };
        _mealRepoMock
            .Setup(r => r.GetByDateAsync(userId, date))
            .ReturnsAsync(fakeMealLog);

        // Symulujemy, że dzisiaj wypił 0ml wody
        _waterRepoMock
            .Setup(r => r.GetByDateAsync(userId, date))
            .ReturnsAsync((WaterLog)null);

        // Act (Wykonanie testowanej logiki biznesowej)
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert (Sprawdzenie, czy algorytm policzył poprawnie)
        int expectedBaseTarget = (int)(70m * 35m); // 2450 ml
        int expectedFinalTarget = expectedBaseTarget + 500; // 2950 ml (bo wysokie białko)

        Assert.Equal(expectedFinalTarget, result.TargetAmountMl);
        Assert.True(result.IsExtraHydrationRequired);
    }
}