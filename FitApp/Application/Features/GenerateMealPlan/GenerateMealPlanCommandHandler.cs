using FitApp.Application.Interfaces; 
using FitApp.Domain.Entities; 
using FitApp.Infrastructure.Interfaces; 
using MediatR;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FitApp.Application.Features.GenerateMealPlan;

public class GenerateMealPlanCommandHandler : IRequestHandler<GenerateMealPlanCommand, Guid>
{
    private readonly IAiService _aiService; // lub IGeminiAiService
    private readonly IMealPlanRepository _repository;

    public GenerateMealPlanCommandHandler(IAiService aiService, IMealPlanRepository repository)
    {
        _aiService = aiService;
        _repository = repository;
    }

    public async Task<Guid> Handle(GenerateMealPlanCommand request, CancellationToken cancellationToken)
    {
        // 1. Instrukcja dla AI (Zmienione Carbohydrates na Carbs, żeby pasowało do bazy)
        string systemPrompt = @"You are a strict diet API. You must ONLY return a valid JSON array of meal items based on the user's prompt. Do NOT wrap it in markdown block quotes (like ```json). Just the raw JSON array.
        Format must be exactly like this:
        [
          {
            ""MealType"": ""Śniadanie"",
            ""ProductName"": ""Piwo jasne pełne"",
            ""Grams"": 500,
            ""Calories"": 215,
            ""Proteins"": 2.5,
            ""Carbs"": 15.0,
            ""Fats"": 0.0
          }
        ]";

        string fullPrompt = $"{systemPrompt}\n\nUser request: {request.Prompt}";

        // 2. Odpytujemy model AI
        string aiResponse = await _aiService.AskCoachAsync(fullPrompt);

        // 3. Czyścimy odpowiedź z ewentualnych znaczników markdown
        aiResponse = aiResponse.Replace("```json", "").Replace("```", "").Trim();

        // 4. Parsujemy JSON
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var generatedItems = JsonSerializer.Deserialize<List<AiMealPlanItemDto>>(aiResponse, options);

        if (generatedItems == null || generatedItems.Count == 0)
            throw new Exception("AI zwróciło pusty plan lub niepoprawny format JSON.");

        // 5. Tworzymy główny obiekt planu
        var mealPlan = new MealPlan
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Prompt = request.Prompt,
            CreatedAt = DateTime.UtcNow,
            Items = new List<MealPlanItem>()
        };

        // 6. Przepisujemy pozycje (z poprawionym mapowaniem Carbs i formatem decimal)
        foreach (var item in generatedItems)
        {
            mealPlan.Items.Add(new MealPlanItem
            {
                Id = Guid.NewGuid(),
                MealPlanId = mealPlan.Id,
                MealType = item.MealType,
                ProductName = item.ProductName,
                Grams = item.Grams,
                Calories = item.Calories,
                Proteins = item.Proteins,
                Carbs = item.Carbs, // Poprawiona nazwa!
                Fats = item.Fats
            });
        }

        // 7. Zapisujemy do bazy danych
        await _repository.AddAsync(mealPlan);

        return mealPlan.Id;
    }
}

// Zaktualizowane DTO pod typy używane w Twojej bazie (decimal zamiast double)
public class AiMealPlanItemDto
{
    public string MealType { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Grams { get; set; }
    public int Calories { get; set; }
    public decimal Proteins { get; set; }
    public decimal Carbs { get; set; } // Poprawione nazewnictwo
    public decimal Fats { get; set; }
}