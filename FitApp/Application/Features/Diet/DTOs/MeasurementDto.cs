namespace FitApp.Application.DTOs;

public class MeasurementDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Weight { get; set; }
    public decimal BodyFatPercentage { get; set; }
}