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
            FatsPer100g = request.FatPer100g
        };

        await _foodRepository.AddAsync(food);
        // Uwaga: Upewnij się, że Twój repoyzotirum wywołuje na końcu await _context.SaveChangesAsync();
        
        return food.Id; // Zwracamy nowo wygenerowane ID!
    }
}