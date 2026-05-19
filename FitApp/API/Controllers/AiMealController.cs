using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FitApp.Application.Features.Diet;

namespace FitApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiMealController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly HttpClient _httpClient;

        public AiMealController(IMediator mediator)
        {
            _mediator = mediator;
            _httpClient = new HttpClient();
        }

        [HttpGet("generate")]
        public async Task<IActionResult> GenerateMeal([FromQuery] double protein, [FromQuery] double carbs, [FromQuery] double fat)
        {
            var query = new GetAiMealQuery 
            { 
                Protein = protein, 
                Carbs = carbs, 
                Fat = fat 
            };
            
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("generate-ai")]
        public async Task<IActionResult> GenerateMealAi([FromQuery] double protein, [FromQuery] double carbs, [FromQuery] double fat)
        {
            var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            
            if (string.IsNullOrEmpty(apiKey))
            {
                return BadRequest("Brak klucza GEMINI_API_KEY w konfiguracji serwera.");
            }

            var prompt = $"Stwórz jeden kreatywny, zdrowy posiłek dla sportowca, który ma dokładnie: " +
                         $"{protein}g białka, {carbs}g węglowodanów i {fat}g tłuszczu. " +
                         $"Podziel go na 2-4 składniki. " +
                         $"Zwróć wynik TYLKO I WYŁĄCZNIE jako czysty, poprawny kod JSON (tablica obiektów), " +
                         $"bez żadnego formatowania markdown (bez ```json ... ```), bez dodatkowego tekstu. " +
                         $"Format ma wyglądać dokładnie tak: [ {{\"name\": \"Nazwa produktu\", \"weightInGrams\": 150}} ]";

            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };

            try
            {
                var jsonRequest = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

              var response = await _httpClient.PostAsync(
    $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash:generateContent?key={apiKey}", 
    content
);

                // TUTAJ JEST ZMIANA - Wyciągamy dokładny błąd od Google!
                if (!response.IsSuccessStatusCode)
                {
                    var googleError = await response.Content.ReadAsStringAsync();
                    var keyPreview = apiKey.Length > 5 ? apiKey.Substring(0, 5) : apiKey;
                    return StatusCode((int)response.StatusCode, $"Odrzucono przez Google. Klucz zaczyna się od: '{keyPreview}...'. Powód: {googleError}");
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

                var mealIngredients = JsonSerializer.Deserialize<object>(aiTextResponse);
                return Ok(mealIngredients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Błąd serwera C#: {ex.Message}");
            }
        }
    }
}