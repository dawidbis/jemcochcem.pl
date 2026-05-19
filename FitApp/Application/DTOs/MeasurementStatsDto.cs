namespace FitApp.Application.DTOs;

public class MeasurementStatsDto
{
    public decimal? LatestWeight { get; set; }
    public decimal? PreviousWeight { get; set; }
    public decimal? WeightChange { get; set; }
    public decimal? CurrentBmi { get; set; }
    public string? BmiCategory { get; set; }
    public decimal? TargetWeight { get; set; }
    public decimal? ProgressPercent { get; set; }
    public int TotalMeasurements { get; set; }
}
