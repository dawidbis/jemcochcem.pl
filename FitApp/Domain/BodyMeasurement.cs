namespace FitApp.Domain.Entities;
public class BodyMeasurement 
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public decimal Weight { get; set; }
    public decimal BodyFatPercentage { get; set; }
}