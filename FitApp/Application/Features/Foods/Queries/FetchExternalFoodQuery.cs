namespace FitApp.Application.Features.Foods;

using MediatR;
using FitApp.Application.DTOs;
using FitApp.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

// Zapytanie przyjmuje tylko kod kreskowy i zwraca FoodDto (lub null, jeśli nie znajdzie)
public record FetchExternalFoodQuery(string Barcode) : IRequest<FoodDto?>;

public class FetchExternalFoodHandler : IRequestHandler<FetchExternalFoodQuery, FoodDto?>
{
    private readonly IOffApiClient _offClient;

    public FetchExternalFoodHandler(IOffApiClient offClient)
    {
        _offClient = offClient;
    }

    public async Task<FoodDto?> Handle(FetchExternalFoodQuery request, CancellationToken ct)
    {
        // Po prostu wywołujemy Twojego klienta!
        var product = await _offClient.FetchProductByBarcode(request.Barcode);
        return product;
    }
}