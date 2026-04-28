namespace FitApp.Domain.Entities;

public class MacroNutrients
{
    public double Protein { get; set; }
    public double Carbs { get; set; }
    public double Fat { get; set; }
    public int Kcal { get; set; }

    public static MacroNutrients Zero()
    {
        return new MacroNutrients { Protein = 0, Carbs = 0, Fat = 0, Kcal = 0 };
    }
}