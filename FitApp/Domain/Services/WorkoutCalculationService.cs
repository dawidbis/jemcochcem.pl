namespace FitApp.Domain.Services;

using FitApp.Domain.Entities;
using FitApp.Domain.Interfaces;

public class WorkoutCalculationService : IWorkoutCalculationService
{
    public decimal CalculateSessionVolume(IEnumerable<SetEntry> sets)
    {
        if (sets is null) return 0m;
        return sets.Sum(s => s.Weight * s.Reps);
    }

    // Epley: 1RM = w * (1 + reps/30). Dla 1 powtórzenia zwraca po prostu ciężar.
    public decimal EstimateOneRepMax(decimal weight, int reps)
    {
        if (reps <= 0) return 0m;
        if (reps == 1) return weight;
        return Math.Round(weight * (1m + reps / 30m), 2);
    }
}
