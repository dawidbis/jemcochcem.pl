namespace FitApp.Application.Features.Diet;

using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FitApp.Application.DTOs;
using FitApp.Infrastructure.Interfaces;

// ==========================================
// 1. ZAPYTANIE: Wyszukiwanie produktów
// ==========================================
public class SearchFoodsQuery : IRequest<IEnumerable<FoodDto>>
{
    public string SearchTerm { get; set; } = string.Empty;
}

public class SearchFoodsHandler : IRequestHandler<SearchFoodsQuery, IEnumerable<FoodDto>>
{
    private readonly IFoodRepository _foodRepository;

    public SearchFoodsHandler(IFoodRepository foodRepository)
    {
        _foodRepository = foodRepository;
    }

    public async Task<IEnumerable<FoodDto>> Handle(SearchFoodsQuery request, CancellationToken ct)
    {
        var foods = await _foodRepository.GetAllAsync(); 

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            // Szukamy po nazwie LUB po kodzie kreskowym
            foods = foods.Where(f => 
                f.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (f.Barcode != null && f.Barcode.Contains(request.SearchTerm))
            ).ToList();
        }

        // Mapowanie na Twój zaktualizowany FoodDto
        return foods.Select(f => new FoodDto
        {
            Id = f.Id, // <-- To pozwoli połączyć produkt z dziennikiem!
            Barcode = f.Barcode ?? string.Empty,
            Name = f.Name,
            CaloriesPer100g = f.CaloriesPer100g,
            Macros = new MacroNutrientsDto
            {
                Protein = f.ProteinPer100g,
                Carbs = f.CarbsPer100g,
                Fats = f.FatsPer100g
            }
        });
    }
}

// ==========================================
// 2. ZAPYTANIE: Pobieranie produktu po ID
// ==========================================
public class GetFoodByIdQuery : IRequest<FoodDto>
{
    public Guid Id { get; set; }
}

public class GetFoodByIdHandler : IRequestHandler<GetFoodByIdQuery, FoodDto>
{
    private readonly IFoodRepository _foodRepository;

    public GetFoodByIdHandler(IFoodRepository foodRepository)
    {
        _foodRepository = foodRepository;
    }

    public async Task<FoodDto> Handle(GetFoodByIdQuery request, CancellationToken ct)
    {
        var food = await _foodRepository.GetByIdAsync(request.Id);

        if (food == null) return null;
         var macros = new MacroNutrientsDto
        {
            Protein = food.ProteinPer100g,
            Carbs = food.CarbsPer100g,
            Fats = food.FatsPer100g
        };
        return new FoodDto
        {
            Id = food.Id,
            Barcode = food.Barcode ?? string.Empty,
            Name = food.Name,
            CaloriesPer100g = food.CaloriesPer100g,
            Macros = macros
        };
    }
}