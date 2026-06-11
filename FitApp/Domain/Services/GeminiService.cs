using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FitApp.Domain.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;

        public GeminiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<object> GenerateAiMealAsync(double protein, double carbs, double fat)
        {
            var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");

            var envPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "FitApp.Client", ".env"));
            
            if (File.Exists(envPath))
            {
                var lines = File.ReadAllLines(envPath);
                foreach (var line in lines)
                {
                    if (line.StartsWith("GEMINI_API_KEY="))
                    {
                        apiKey = line.Substring("GEMINI_API_KEY=".Length);
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("Błąd: Nie znaleziono klucza API w pliku .env.");
            }

            // BRUTALNE CZYSZCZENIE KLUCZA: Wywala wszystkie "niewidzialne" znaki, spacje i entery
            apiKey = Regex.Replace(apiKey, @"\s+", "");
            apiKey = apiKey.Replace("\"", "").Replace("'", "");

            var prompt = "Jesteś profesjonalnym dietetykiem i kucharzem sportowym. " +
                         $"Stwórz kreatywny, zdrowy i pyszny posiłek, który ma DOKŁADNIE: " +
                         $"{protein}g białka, {carbs}g węglowodanów i {fat}g tłuszczu. " +
                         "Zwróć wynik TYLKO I WYŁĄCZNIE jako czysty kod JSON. " +
                         "JSON ma mieć dokładnie taką strukturę: " +
                         "{ " +
                         "\"mealName\": \"Chwytliwa nazwa posiłku\", " +
                         "\"ingredients\": [ " +
                         "{ \"name\": \"Nazwa składnika 1\", \"weightInGrams\": 100 } " +
                         "], " +
                         "\"preparationSteps\": [ " +
                         "\"Krok 1 przygotowania...\", " +
                         "\"Krok 2 przygotowania...\" " +
                         "] " +
                         "}";

            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };

            var jsonRequest = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

            var response = await _httpClient.PostAsync(requestUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var googleError = await response.Content.ReadAsStringAsync();
                throw new Exception($"Odrzucono przez Google. Powód: {googleError}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            
            using var doc = JsonDocument.Parse(jsonResponse);
            var aiTextResponse = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString()?.Trim();

            if (aiTextResponse != null && aiTextResponse.StartsWith("```"))
            {
                aiTextResponse = aiTextResponse.Replace("```json", "").Replace("```", "").Trim();
            }

            return JsonSerializer.Deserialize<object>(aiTextResponse);
        }
    }
}