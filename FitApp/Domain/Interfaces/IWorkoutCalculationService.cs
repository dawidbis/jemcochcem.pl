namespace FitApp.Domain.Interfaces;

using FitApp.Domain.Entities;

public interface IWorkoutCalculationService
{
    // Tonaż sesji = suma (ciężar * powtórzenia) ze wszystkich serii
    decimal CalculateSessionVolume(IEnumerable<SetEntry> sets);

    // Szacowany ciężar maksymalny 1RM (wzór Epleya). Przyda się pod wykres progresji.
    decimal EstimateOneRepMax(decimal weight, int reps);
}
