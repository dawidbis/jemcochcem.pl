namespace FitApp.Domain.Services;

using System;

public class TdeeCalculationService 
{
    public decimal CalculateBMR(decimal weight, decimal height, int age, string gender) 
    {
        // Równanie Mifflin-St Jeor
        var baseBmr = (10m * weight) + (6.25m * height) - (5m * age);
        return gender.Equals("male", StringComparison.OrdinalIgnoreCase) 
            ? baseBmr + 5m 
            : baseBmr - 161m;
    }

    public int CalculateTDEE(decimal weight, decimal height, int age, string gender, decimal activityLevel) 
    {
        var bmr = CalculateBMR(weight, height, age, gender);
        return (int)Math.Round(bmr * activityLevel);
    }
}