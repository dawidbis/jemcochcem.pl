namespace FitApp.Infrastructure.ExternalServices;

using FitApp.Application.Interfaces;
using FitApp.Application.DTOs;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;

public class AiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    // Wstrzykujemy HttpClient oraz IConfiguration do pobrania klucza API z appsettings.json
    public AiService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["Gemini:ApiKey"] 
    ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY") 
    ?? throw new Exception("Brak klucza API Gemini!");  }

    public Task<AiAnalysisResult> AnalyzeMealPhotoAsync(byte[] photoBytes) => throw new NotImplementedException();

    public async Task<string> AskCoachAsync(string prompt)
{
    var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.1-flash-lite:generateContent?key={_apiKey}";
    
    var requestBody = new
    {
        contents = new[]
        {
            new 
            { 
                role = "user",
                parts = new[] { new { text = prompt } } 
            }
        }
    };

    var jsonBody = JsonSerializer.Serialize(requestBody);
    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

    var response = await _httpClient.PostAsync(requestUrl, content);
    var responseJson = await response.Content.ReadAsStringAsync();

    // KROK 1: Jeśli Google zwróciło status błędu (np. 400, 403, 500)
    if (!response.IsSuccessStatusCode)
    {
        throw new Exception($"Błąd Gemini API (Status: {response.StatusCode}). Treść odpowiedzi: {responseJson}");
    }

    // KROK 2: Bezpieczne parsowanie JSON z przechwytywaniem wyjątków
    try
    {
        using var document = JsonDocument.Parse(responseJson);

        if (document.RootElement.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
        {
            var firstCandidate = candidates[0];

            if (firstCandidate.TryGetProperty("finishReason", out var finishReason) && finishReason.GetString() == "SAFETY")
            {
                return "Nasz dietetyk AI nie może wygenerować planu opartego o te produkty ze względów zdrowotnych.";
            }

            if (firstCandidate.TryGetProperty("content", out var contentElement) &&
                contentElement.TryGetProperty("parts", out var parts) &&
                parts.GetArrayLength() > 0 &&
                parts[0].TryGetProperty("text", out var textProp))
            {
                return textProp.GetString() ?? string.Empty;
            }
        }
    }
    catch (JsonException)
    {
        throw new Exception($"Odpowiedź od Google nie jest poprawnym JSON-em! Surowa treść: {responseJson}");
    }

    return "Nie udało się wygenerować planu diety. Spróbuj sformułować zapytanie inaczej.";
}
}