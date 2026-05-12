namespace FitApp.Application.Features.Diet;

using MediatR;
using FitApp.Application.Interfaces; // Dla IOffApiClient
using FitApp.Domain.Interfaces;
using FitApp.Infrastructure.Interfaces;
using FitApp.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

// Użytkownik podaje kod kreskowy zamiast ID produktu
public record AddExternalMealItemCommand(Guid UserId, DateTime Date, string Barcode, decimal Grams) : IRequest<Unit>;

public class AddExternalMealItemHandler : IRequestHandler<AddExternalMealItemCommand, Unit>
{
    private readonly IOffApiClient _offClient;
    private readonly IFoodRepository _foodRepository;
    private readonly IMediator _mediator; // Użyjemy mediatora, by wywołać już istniejącą logikę dodawania

    public AddExternalMealItemHandler(
        IOffApiClient offClient, 
        IFoodRepository foodRepository, 
        IMediator mediator)
    {
        _offClient = offClient;
        _foodRepository = foodRepository;
        _mediator = mediator;
    }

    public async Task<Unit> Handle(AddExternalMealItemCommand request, CancellationToken ct)
    {
        // 1. Sprawdzamy, czy produkt już jest w naszej bazie (by nie duplikować)
        var existingProduct = await _foodRepository.GetByBarcodeAsync(request.Barcode);
        Guid productId;

        if (existingProduct != null)
        {
            productId = existingProduct.Id;
        }
        else
        {
            // 2. Pobieramy dane z OpenFoodFacts
            var externalProduct = await _offClient.FetchProductByBarcode(request.Barcode);
            if (externalProduct == null) throw new Exception("Nie znaleziono produktu w bazie OFF.");

            // 3. Mapujemy DTO z API na naszą encję bazodanową FoodProduct
            var newProduct = new FoodProduct
            {
                Id = Guid.NewGuid(),
                Barcode = request.Barcode,
                Name = externalProduct.Name,
                CaloriesPer100g = externalProduct.CaloriesPer100g,
                ProteinPer100g = externalProduct.Macros.Protein,
                CarbsPer100g = externalProduct.Macros.Carbs,
                FatsPer100g = externalProduct.Macros.Fats
            };

            // 4. Zapisujemy produkt na stałe w naszej bazie
            await _foodRepository.AddAsync(newProduct);
            productId = newProduct.Id;
        }

        // 5. Skoro produkt jest już w bazie, używamy Twojego gotowego AddMealItemCommand!
        // Wykorzystujemy re-używalność kodu.
        await _mediator.Send(new AddMealItemCommand
        {
            UserId = request.UserId,
            Date = request.Date,
            FoodProductId = productId,
            Grams = request.Grams
        });

        return Unit.Value;
    }
}