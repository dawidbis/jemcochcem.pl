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
    
    // 1. Dodajemy serwis do obliczeń!
    private readonly INutritionCalculationService _nutritionService;

    public GetDailyDiaryHandler(
        IMealLogRepository mealLogRepository, 
        INutritionCalculationService nutritionService) // <-- Wstrzykujemy go
    {
        _mealLogRepository = mealLogRepository;
        _nutritionService = nutritionService;
    }

    public async Task<DiaryDto> Handle(GetDailyDiaryQuery request, CancellationToken ct)
    {
        var log = await _mealLogRepository.GetByDateAsync(request.UserId, request.Date);

        if (log == null) 
        {
            return new DiaryDto { Date = request.Date };
        }

        return new DiaryDto
        {
            Date = log.Date,
            TotalCalories = log.TotalCalories,
            // Ponieważ Twój MealLogDomainService przelicza też makro dla całego dnia, 
            // możemy je tu od razu podpiąć! (jeśli masz je w encji MealLog)
            TotalProtein = log.TotalProtein, 
            TotalCarbs = log.TotalCarbs,
            TotalFats = log.TotalFats,

            Items = log.Items.Select(item => 
            {
                ArgumentNullException.ThrowIfNull(item.FoodProduct); 

                // 2. Używamy serwisu do policzenia makro DLA TEGO KONKRETNEGO POSIŁKU
                var calculatedMacros = _nutritionService.CalculateItemMacros(
                    item.Grams, 
                    item.FoodProduct.ProteinPer100g, 
                    item.FoodProduct.CarbsPer100g, 
                    item.FoodProduct.FatsPer100g
                );

                return new MealLogItemDto
                {
                    Id = item.Id,
                    FoodName = item.FoodProduct.Name,
                    Grams = item.Grams,
                    // 3. Używamy serwisu zamiast ręcznej matematyki
                    Calories = _nutritionService.CalculateItemCalories(item.Grams, item.FoodProduct.CaloriesPer100g),
                    
                    // 4. PRZYPISUJEMY POLICZONE MAKRO (koniec z nullem w Swaggerze!)
                    Macros = new MacroNutrientsDto 
                    {
                        Protein = calculatedMacros.Protein,
                        Carbs = calculatedMacros.Carbs,
                        Fats = calculatedMacros.Fats
                    }
                };
            }).ToList()
        };
    }
}