namespace FitApp.Application.Features.Diet;

using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using FitApp.Domain.Entities;
using FitApp.Infrastructure.Interfaces;

// 1. Model danych przychodzących (Komenda)
public class CreateFoodCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public int CaloriesPer100g { get; set; }
    public decimal ProteinPer100g { get; set; }
    public decimal CarbsPer100g { get; set; }
    public decimal FatPer100g { get; set; }

    // Mikroskładniki (na 100g)
    public decimal FiberPer100g { get; set; }
    public decimal SugarsPer100g { get; set; }
    public decimal SaturatedFatPer100g { get; set; }
    public decimal SodiumPer100g { get; set; }
    public decimal CalciumPer100g { get; set; }
    public decimal IronPer100g { get; set; }
}

// 2. Handler, który zapisuje to do bazy
public class CreateFoodHandler : IRequestHandler<CreateFoodCommand, Guid>
{
    private readonly IFoodRepository _foodRepository;

    public CreateFoodHandler(IFoodRepository foodRepository)
    {
        _foodRepository = foodRepository;
    }

    public async Task<Guid> Handle(CreateFoodCommand request, CancellationToken ct)
    {
        var food = new FoodProduct
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Barcode = request.Barcode,
            CaloriesPer100g = request.CaloriesPer100g,
            ProteinPer100g = request.ProteinPer100g,
            CarbsPer100g = request.CarbsPer100g,
            FatsPer100g = request.FatPer100g,
            FiberPer100g = request.FiberPer100g,
            SugarsPer100g = request.SugarsPer100g,
            SaturatedFatPer100g = request.SaturatedFatPer100g,
            SodiumPer100g = request.SodiumPer100g,
            CalciumPer100g = request.CalciumPer100g,
            IronPer100g = request.IronPer100g
        };

        await _foodRepository.AddAsync(food);
        // Uwaga: Upewnij się, że Twój repoyzotirum wywołuje na końcu await _context.SaveChangesAsync();
        
        return food.Id; // Zwracamy nowo wygenerowane ID!
    }
}