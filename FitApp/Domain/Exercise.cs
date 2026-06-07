namespace FitApp.Domain.Entities;

public class Exercise
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Grupa mięśniowa jako string (np. "Klatka", "Plecy", "Nogi", "Barki", "Ramiona", "Brzuch", "Cardio")
    // Trzymamy string zamiast enuma -> prostsze mapowanie w Postgres i łatwo dodać własne kategorie.
    public string MuscleGroup { get; set; } = string.Empty;

    // false = ćwiczenie globalne (wbudowane, seed), true = dodane przez użytkownika
    public bool IsCustom { get; set; }

    // Wypełnione tylko dla ćwiczeń własnych (IsCustom == true). Globalne mają null.
    public Guid? UserId { get; set; }

    public Exercise() { }

    public Exercise(string name, string muscleGroup, bool isCustom = false, Guid? userId = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        MuscleGroup = muscleGroup;
        IsCustom = isCustom;
        UserId = userId;
    }
}
