using System;

namespace FitApp.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public CalorieGoal Goal { get; set; } = new CalorieGoal();

    public void SetNewGoal(CalorieGoal goal)
    {
        Goal = goal;
    }
}