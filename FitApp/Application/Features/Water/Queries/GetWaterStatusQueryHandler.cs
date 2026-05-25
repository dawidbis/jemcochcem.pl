using FitApp.Infrastructure.Interfaces;
using MediatR;

public class GetWaterStatusQueryHandler : IRequestHandler<GetWaterStatusQuery, WaterStatusDto>
{
    private readonly IWaterLogRepository _waterRepo;
    private readonly IMealLogRepository _mealRepo;
    private readonly IBodyMeasurementRepository _measurementRepo; // Do pobrania wagi

    public GetWaterStatusQueryHandler(IWaterLogRepository waterRepo, IMealLogRepository mealRepo, IBodyMeasurementRepository measurementRepo)
    {
        _waterRepo = waterRepo;
        _mealRepo = mealRepo;
        _measurementRepo = measurementRepo;
    }

    public async Task<WaterStatusDto> Handle(GetWaterStatusQuery request, CancellationToken cancellationToken)
    {
        // 1. Pobierz obecny stan wypitej wody
        var waterLog = await _waterRepo.GetByDateAsync(request.UserId, request.Date);
        int currentAmount = waterLog?.AmountMl ?? 0;

        // 2. Wylicz cel bazowy na podstawie ostatniej wagi użytkownika (domyślnie 70kg jeśli brak danych)
        var lastMeasurement = await _measurementRepo.GetLatestAsync(request.UserId);
        decimal weight = lastMeasurement?.Weight ?? 70m;
        int targetAmount = (int)(weight * 35m);

        // 3. Sprawdź zawartość talerza (MealLog) z dzisiaj
        var mealLog = await _mealRepo.GetByDateAsync(request.UserId, request.Date);
        bool isExtraHydrationRequired = false;
        string alertMessage = "Twój poziom nawodnienia jest w normie.";

        if (mealLog != null)
        {
            // Przykładowe progi inżynierskie: białko > 140g
            if (mealLog.TotalProtein > 140)
            {
                targetAmount += 500; // Zwiększamy cel o 500ml
                isExtraHydrationRequired = true;
                alertMessage = "Wykryto wysokie spożycie białka w diecie! Cel nawodnienia zwiększony o 500ml w celu ochrony nerek.";
            }
        }

        return new WaterStatusDto(currentAmount, targetAmount, isExtraHydrationRequired, alertMessage);
    }
}