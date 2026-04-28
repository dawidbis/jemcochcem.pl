using System;

namespace FitApp.Domain.Entities;

public class BodyMeasurement
{
    public DateTime MeasuredAt { get; set; }
    public double Weight { get; set; }
    public double BodyFat { get; set; }
}