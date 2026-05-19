namespace FitApp.Application.DTOs;

public class MeasurementDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Weight { get; set; }
    public decimal? BodyFatPercentage { get; set; }
    public decimal? Waist { get; set; }
    public decimal? Hips { get; set; }
    public string? Notes { get; set; }
    public decimal? Bmi { get; set; }
}
