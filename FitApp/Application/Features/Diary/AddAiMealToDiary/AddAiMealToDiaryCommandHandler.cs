using FitApp.Domain.Entities;
using FitApp.Infrastructure.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FitApp.Application.Features.Diary.AddAiMealToDiary;

public class AddAiMealToDiaryCommandHandler
    : IRequestHandler<AddAiMealToDiaryCommand, Guid>
{
    private readonly IMealPlanRepository _mealPlanRepo;
    private readonly IFoodRepository _foodRepo;
    private readonly IMealLogRepository _mealLogRepo;

    private readonly IDistributedCache _cache;
    public AddAiMealToDiaryCommandHandler(
        IMealPlanRepository mealPlanRepo,
        IFoodRepository foodRepo,
        IMealLogRepository mealLogRepo,
        IDistributedCache cache)
    {
        _mealPlanRepo = mealPlanRepo;
        _foodRepo = foodRepo;
        _mealLogRepo = mealLogRepo;
        _cache = cache;
    }

    public async Task<Guid> Handle(
        AddAiMealToDiaryCommand request,
        CancellationToken cancellationToken)
    {
        var planItem = await _mealPlanRepo.GetItemByIdAsync(request.MealPlanItemId);
        if (planItem == null) throw new Exception("Nie znaleziono pozycji w planie.");

        Guid foodProductId = planItem.FoodProductId 
            ?? await CreateFoodProductFromDomainItem(planItem);

        var mealLog = await _mealLogRepo.GetByDateAsync(request.UserId, request.Date.Date);

        if (mealLog == null)
        {
            mealLog = new MealLog
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Date = request.Date.Date,
                Items = new List<MealLogItem>()
            };
            await _mealLogRepo.AddAsync(mealLog);
        }

        var item = CreateMealLogItem(mealLog.Id, foodProductId, (int)planItem.Grams);
        
        // 1. Dodajemy pozycję
        await _mealLogRepo.AddMealLogItemAsync(item);

        // 2. PRZELICZAMY SUMY (Musisz pobrać produkt, żeby znać jego wartości odżywcze)
        var food = await _foodRepo.GetByIdAsync(foodProductId);
        
        // Obliczamy wartości dla tego jednego dodanego składnika
        decimal multiplier = planItem.Grams / 100m;
        mealLog.TotalCalories += (int)(food.CaloriesPer100g * multiplier);
        mealLog.TotalProtein += food.ProteinPer100g * multiplier;
        mealLog.TotalCarbs += food.CarbsPer100g * multiplier;
        mealLog.TotalFats += food.FatsPer100g * multiplier;

        // Mikroskładniki (spójne z resztą sumowania – produkty AI mają domyślnie 0)
        mealLog.TotalFiber += food.FiberPer100g * multiplier;
        mealLog.TotalSugars += food.SugarsPer100g * multiplier;
        mealLog.TotalSaturatedFat += food.SaturatedFatPer100g * multiplier;
        mealLog.TotalSodium += food.SodiumPer100g * multiplier;
        mealLog.TotalCalcium += food.CalciumPer100g * multiplier;
        mealLog.TotalIron += food.IronPer100g * multiplier;

        // 3. Zapisujemy zmiany w MealLog (zaktualizowane sumy)
        // Zakładam, że masz metodę Update w swoim repozytorium
        await _mealLogRepo.UpdateAsync(mealLog);
        await _mealLogRepo.SaveChangesAsync();
        string cacheKey = $"diary:{request.UserId}:{request.Date:yyyy-MM-dd}";
        await _cache.RemoveAsync(cacheKey, cancellationToken);
        return item.Id;
    }
    private MealLogItem CreateMealLogItem(
        Guid mealLogId,
        Guid foodProductId,
        int grams)
    {
        return new MealLogItem
        {
            Id = Guid.NewGuid(),
            MealLogId = mealLogId,
            FoodProductId = foodProductId,
            Grams = grams
        };
    }

    private async Task<Guid>
        CreateFoodProductFromDomainItem(
            MealPlanItem item)
    {
        var multiplier =
            item.Grams > 0
                ? (100m / item.Grams)
                : 1m;

        var newFood = new FoodProduct
        {
            Id = Guid.NewGuid(),
            Name = $"{item.ProductName} (AI)",
            Barcode = "",

            CaloriesPer100g =
                (int)(item.Calories * multiplier),

            ProteinPer100g =
                item.Proteins * multiplier,

            CarbsPer100g =
                item.Carbs * multiplier,

            FatsPer100g =
                item.Fats * multiplier
        };

        await _foodRepo.AddAsync(newFood);
        return newFood.Id;
    }
}