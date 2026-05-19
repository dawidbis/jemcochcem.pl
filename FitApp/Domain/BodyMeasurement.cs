namespace FitApp.Domain.Entities;
public class BodyMeasurement
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public decimal Weight { get; set; }
    public decimal? BodyFatPercentage { get; set; }
    public decimal? Waist { get; set; }
    public decimal? Hips { get; set; }
    public string? Notes { get; set; }
}
