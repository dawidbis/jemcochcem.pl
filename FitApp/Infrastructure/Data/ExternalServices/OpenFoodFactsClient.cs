namespace FitApp.Infrastructure.ExternalServices;

using FitApp.Application.Interfaces;
using FitApp.Application.DTOs;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using FitApp.Infrastructure.Interfaces;
using System;

public class OpenFoodFactsClient : IOffApiClient
{
    private readonly HttpClient _httpClient;

    public OpenFoodFactsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        // Opcjonalnie: ustawienie bazowego adresu dla wygody
        _httpClient.BaseAddress = new Uri("https://world.openfoodfacts.org/api/v0/");
    }

    public async Task<FoodDto?> FetchProductByBarcode(string barcode)
    {
        // 1. Wysyłamy zapytanie GET do API OpenFoodFacts
        var response = await _httpClient.GetAsync($"product/{barcode}.json");
        
        if (!response.IsSuccessStatusCode)
            return null; // API nie odpowiedziało poprawnie

        // 2. Odczytujemy odpowiedź jako string
        var jsonString = await response.Content.ReadAsStringAsync();

        // 3. Parsujemy JSON za pomocą JsonDocument (żeby nie tworzyć wielkich klas)
        using var document = JsonDocument.Parse(jsonString);
        var root = document.RootElement;

        // OFF API zwraca status: 0 jeśli nie znalazł produktu, 1 jeśli znalazł
        if (root.TryGetProperty("status", out var statusElement) && statusElement.GetInt32() == 0)
        {
            return null; // Nie znaleziono produktu o tym kodzie kreskowym
        }

        // 4. Dobieramy się do zagnieżdżonych obiektów "product" i "nutriments"
        if (!root.TryGetProperty("product", out var productElement)) return null;
        if (!productElement.TryGetProperty("nutriments", out var nutrimentsElement)) return null;

        // 5. Mapujemy dane tworząc najpierw obiekt Makro, a potem Produkt
        var macros = new MacroNutrientsDto
        {
            Protein = GetDecimalSafe(nutrimentsElement, "proteins_100g"),
            Carbs = GetDecimalSafe(nutrimentsElement, "carbohydrates_100g"),
            Fats = GetDecimalSafe(nutrimentsElement, "fat_100g")
        };

        return new FoodDto
        {
            Id = Guid.Empty, // Pozwalamy, by to Handler/Baza ułożyła własne ID przy zapisie
            Barcode = barcode,
            Name = GetStringSafe(productElement, "product_name") ?? "Nieznany produkt",
            CaloriesPer100g = GetIntSafe(nutrimentsElement, "energy-kcal_100g"),
            Macros = macros // Podpinamy wyliczone makro!
        };
        
    }

    // --- Metody pomocnicze do bezpiecznego wyciągania danych z JSONa OFF API ---
    // (OFF API potrafi zwracać dziwne typy albo brakować właściwości, stąd to zabezpieczenie)

    private string? GetStringSafe(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.String 
            ? prop.GetString() : null;
    }

    private int GetIntSafe(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.Number 
            ? prop.GetInt32() : 0;
    }

    private decimal GetDecimalSafe(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.Number 
            ? prop.GetDecimal() : 0m;
    }
}