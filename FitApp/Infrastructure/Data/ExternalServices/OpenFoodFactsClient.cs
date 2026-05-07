namespace FitApp.Infrastructure.ExternalServices;

using FitApp.Application.Interfaces;
using FitApp.Application.DTOs;
using System.Net.Http;
using System.Threading.Tasks;
using FitApp.Infrastructure.Interfaces;

public class OpenFoodFactsClient : IOffApiClient
{
    private readonly HttpClient _httpClient;

    public OpenFoodFactsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<FoodDto> FetchProductByBarcode(string barcode) => throw new NotImplementedException();
}