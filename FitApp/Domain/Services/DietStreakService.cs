using FitApp.Domain.Entities;
using System;

public class DietStreakService : IDietStreakService
{
    private const decimal AllowedError = 150m; // Margines błędu +/- 150 kcal

    public void CalculateStreak(User user, decimal consumedCalories, decimal targetCalories, DateTime evaluationDate)
    {
        var targetDate = evaluationDate.Date;

        // Blokada: Tylko jedna aktualizacja na dzień kalendarzowy
        if (user.LastStreakUpdate?.Date == targetDate) return;

        bool isCaloriesWithinRange = Math.Abs(targetCalories - consumedCalories) <= AllowedError;

        if (isCaloriesWithinRange)
        {
            // Pierwszy dzień lub kontynuacja (wczoraj)
            if (user.CurrentStreak == 0 || (user.LastStreakUpdate.HasValue && (targetDate - user.LastStreakUpdate.Value.Date).Days == 1))
            {
                user.CurrentStreak++;
            }
            // Powrót po przerwie dłuższej niż 1 dzień
            else if (user.LastStreakUpdate.HasValue && (targetDate - user.LastStreakUpdate.Value.Date).Days > 1)
            {
                user.CurrentStreak = 1;
            }

            // Rekord życiowy
            if (user.CurrentStreak > user.LongestStreak)
            {
                user.LongestStreak = user.CurrentStreak;
            }
        }
        else
        {
            // Dieta zerwana
            user.CurrentStreak = 0;
        }

        user.LastStreakUpdate = targetDate;
    }
}