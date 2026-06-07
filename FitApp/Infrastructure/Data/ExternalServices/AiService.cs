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
       var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}";
        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            }
        };

        var jsonBody = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(requestUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync();
            throw new Exception($"Błąd Gemini API: {response.StatusCode} - {errorText}");
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(responseJson);
    
        // Ścieżka parsowania dla Gemini:
        return document.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? string.Empty;
    }
}