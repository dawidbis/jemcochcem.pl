namespace Backend.dtos;
public class AiResultDto
{
    public string FoodName { get; set; } = string.Empty;
    public double EstimatedWeightGrams { get; set; }
    public int Calories { get; set; }
    public NutrientDto Nutrients { get; set; } = new();
    public string ConfidenceScore { get; set; } = string.Empty; 
}

public class NutrientDto
{
    public double Protein { get; set; }
    public double Carbs { get; set; }
    public double Fat { get; set; }
}