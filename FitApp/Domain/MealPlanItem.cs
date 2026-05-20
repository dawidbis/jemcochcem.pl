using System.Text.Json.Serialization;

namespace FitApp.Domain.Entities;

public class MealPlanItem
{
    public Guid Id { get; set; }
    public Guid MealPlanId { get; set; }

    public string MealType { get; set; } = string.Empty; // np. "Śniadanie", "Obiad"
    
    // Surowe dane wygenerowane przez AI
    public string ProductName { get; set; } = string.Empty; 
    public decimal Grams { get; set; }
    
    // Makro wyestymowane przez AI dla tej konkretnej porcji:
    public int Calories { get; set; }
    public decimal Proteins { get; set; }
    public decimal Carbs { get; set; }
    public decimal Fats { get; set; }

    // Opcjonalne powiązanie z istniejącym produktem z bazy
    // (przydatne, jeśli uda nam się połączyć nazwę z AI z produktem z Twojej bazy)
    public Guid? FoodProductId { get; set; }
    public FoodProduct? FoodProduct { get; set; }
    [JsonIgnore]
    public MealPlan? MealPlan { get; set; }
}