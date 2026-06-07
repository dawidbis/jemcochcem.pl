namespace FitApp.Domain.Entities;
public class FoodProduct 
{
    public Guid Id { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int CaloriesPer100g { get; set; }
    public decimal ProteinPer100g { get; set; }
    public decimal CarbsPer100g { get; set; }
    public decimal FatsPer100g { get; set; }

    // Mikroskładniki (na 100g). Błonnik/cukry/tłuszcze nasycone w gramach, sód/wapń/żelazo w mg.
    public decimal FiberPer100g { get; set; }
    public decimal SugarsPer100g { get; set; }
    public decimal SaturatedFatPer100g { get; set; }
    public decimal SodiumPer100g { get; set; }
    public decimal CalciumPer100g { get; set; }
    public decimal IronPer100g { get; set; }
}