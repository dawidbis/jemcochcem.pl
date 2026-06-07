namespace FitApp.Application.Features.Diet;

using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FitApp.Application.DTOs;
using FitApp.Infrastructure.Interfaces;
using FitApp.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

public class GetDailyDiaryQuery : IRequest<DiaryDto>
{
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
}

public class GetDailyDiaryHandler : IRequestHandler<GetDailyDiaryQuery, DiaryDto>
{
    private readonly IMealLogRepository _mealLogRepository;
    
    // 1. Dodajemy serwis do obliczeń!
    private readonly INutritionCalculationService _nutritionService;
    private readonly IDistributedCache _cache;

    public GetDailyDiaryHandler(
        IMealLogRepository mealLogRepository, 
        INutritionCalculationService nutritionService,
        IDistributedCache cache) // <-- Wstrzykujemy go
    {
        _mealLogRepository = mealLogRepository;
        _nutritionService = nutritionService;
        _cache=cache;
    }

    public async Task<DiaryDto> Handle(GetDailyDiaryQuery request, CancellationToken ct)
    {
        // Klucz unikalny dla usera i konkretnego dnia (format: diary:GUID:yyyy-MM-dd)
        string cacheKey = $"diary:{request.UserId}:{request.Date:yyyy-MM-dd}";

        // 1. Spróbuj pobrać dane z Redisa
        var cachedJson = await _cache.GetStringAsync(cacheKey, ct);
        if (!string.IsNullOrEmpty(cachedJson))
        {
            // Jeśli są w pamięci, deserializuj i zwróć natychmiast (baza Postgres odpoczywa)
            return JsonSerializer.Deserialize<DiaryDto>(cachedJson)!;
        }

        // 2. Jeśli brak w cache (Cache Miss), odpytaj bazę Postgres
        var log = await _mealLogRepository.GetByDateAsync(request.UserId, request.Date);

        if (log == null) 
        {
            return new DiaryDto { Date = request.Date };
        }

        var diaryDto = new DiaryDto
        {
            Date = log.Date,
            TotalCalories = log.TotalCalories,
            TotalProtein = log.TotalProtein,
            TotalCarbs = log.TotalCarbs,
            TotalFats = log.TotalFats,

            TotalFiber = log.TotalFiber,
            TotalSugars = log.TotalSugars,
            TotalSaturatedFat = log.TotalSaturatedFat,
            TotalSodium = log.TotalSodium,
            TotalCalcium = log.TotalCalcium,
            TotalIron = log.TotalIron,

            Items = log.Items.Select(item =>
            {
                ArgumentNullException.ThrowIfNull(item.FoodProduct);

                var calculatedMacros = _nutritionService.CalculateItemMacros(
                    item.Grams,
                    item.FoodProduct.ProteinPer100g,
                    item.FoodProduct.CarbsPer100g,
                    item.FoodProduct.FatsPer100g
                );

                var calculatedMicros = _nutritionService.CalculateItemMicros(
                    item.Grams,
                    item.FoodProduct.FiberPer100g,
                    item.FoodProduct.SugarsPer100g,
                    item.FoodProduct.SaturatedFatPer100g,
                    item.FoodProduct.SodiumPer100g,
                    item.FoodProduct.CalciumPer100g,
                    item.FoodProduct.IronPer100g
                );

                return new MealLogItemDto
                {
                    Id = item.Id,
                    FoodName = item.FoodProduct.Name,
                    Grams = item.Grams,
                    Calories = _nutritionService.CalculateItemCalories(item.Grams, item.FoodProduct.CaloriesPer100g),

                    Macros = new MacroNutrientsDto
                    {
                        Protein = calculatedMacros.Protein,
                        Carbs = calculatedMacros.Carbs,
                        Fats = calculatedMacros.Fats
                    },

                    Micros = new MicroNutrientsDto
                    {
                        Fiber = calculatedMicros.Fiber,
                        Sugars = calculatedMicros.Sugars,
                        SaturatedFat = calculatedMicros.SaturatedFat,
                        Sodium = calculatedMicros.Sodium,
                        Calcium = calculatedMicros.Calcium,
                        Iron = calculatedMicros.Iron
                    }
                };
            }).ToList()
        };

        // 3. Zapisz wygenerowane DTO do Redisa na 15 minut przed zwróceniem
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
        };
        
        var serializedData = JsonSerializer.Serialize(diaryDto);
        await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions, ct);

        return diaryDto;
    }
    }
