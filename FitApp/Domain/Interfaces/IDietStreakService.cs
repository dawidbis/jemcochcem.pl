using FitApp.Domain.Entities;
public interface IDietStreakService
{
    void CalculateStreak(User user, decimal consumedCalories, decimal targetCalories, DateTime evaluationDate);
}