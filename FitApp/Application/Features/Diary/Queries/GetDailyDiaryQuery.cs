namespace FitApp.Application.Features.Diet;

using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FitApp.Application.DTOs;
using FitApp.Infrastructure.Interfaces;
using FitApp.Domain.Interfaces;

public class GetDailyDiaryQuery : IRequest<DiaryDto>
{
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
}

public class GetDailyDiaryHandler : IRequestHandler<GetDailyDiaryQuery, DiaryDto>
{
    private readonly IMealLogRepository _mealLogRepository;
    private readonly INutritionCalculationService _nutritionService;

    public GetDailyDiaryHandler(
        IMealLogRepository mealLogRepository,
        INutritionCalculationService nutritionService)
    {
        _mealLogRepository = mealLogRepository;
        _nutritionService = nutritionService;
    }

    public async Task<DiaryDto> Handle(GetDailyDiaryQuery request, CancellationToken ct)
    {
        var log = await _mealLogRepository.GetByDateAsync(request.UserId, request.Date);

        if (log == null)
            return new DiaryDto { Date = request.Date };

        return new DiaryDto
        {
            Date             = log.Date,
            TotalCalories    = log.TotalCalories,
            TotalProtein     = log.TotalProtein,
            TotalCarbs       = log.TotalCarbs,
            TotalFats        = log.TotalFats,
            TotalFiber       = log.TotalFiber,
            TotalSugars      = log.TotalSugars,
            TotalSaturatedFat= log.TotalSaturatedFat,
            TotalSodium      = log.TotalSodium,
            TotalCalcium     = log.TotalCalcium,
            TotalIron        = log.TotalIron,
            Items = log.Items.Select(item =>
            {
                ArgumentNullException.ThrowIfNull(item.FoodProduct);

                var macros = _nutritionService.CalculateItemMacros(
                    item.Grams,
                    item.FoodProduct.ProteinPer100g,
                    item.FoodProduct.CarbsPer100g,
                    item.FoodProduct.FatsPer100g);

                var micros = _nutritionService.CalculateItemMicros(
                    item.Grams,
                    item.FoodProduct.FiberPer100g,
                    item.FoodProduct.SugarsPer100g,
                    item.FoodProduct.SaturatedFatPer100g,
                    item.FoodProduct.SodiumPer100g,
                    item.FoodProduct.CalciumPer100g,
                    item.FoodProduct.IronPer100g);

                return new MealLogItemDto
                {
                    Id       = item.Id,
                    FoodName = item.FoodProduct.Name,
                    Grams    = item.Grams,
                    Calories = _nutritionService.CalculateItemCalories(item.Grams, item.FoodProduct.CaloriesPer100g),
                    Macros   = new MacroNutrientsDto { Protein = macros.Protein, Carbs = macros.Carbs, Fats = macros.Fats },
                    Micros   = new MicroNutrientsDto { Fiber = micros.Fiber, Sugars = micros.Sugars, SaturatedFat = micros.SaturatedFat, Sodium = micros.Sodium, Calcium = micros.Calcium, Iron = micros.Iron }
                };
            }).ToList()
        };
    }
}
