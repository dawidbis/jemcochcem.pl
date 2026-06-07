namespace FitApp.Domain.Entities;

public class SetEntry
{
    public Guid Id { get; set; }

    public Guid WorkoutSessionId { get; set; }
    public WorkoutSession? WorkoutSession { get; set; }

    public Guid ExerciseId { get; set; }
    public Exercise? Exercise { get; set; }

    public int SetNumber { get; set; }   // numer serii w ramach ćwiczenia (1, 2, 3...)
    public decimal Weight { get; set; }   // ciężar w kg
    public int Reps { get; set; }         // liczba powtórzeń

    public SetEntry() { }

    public SetEntry(Guid exerciseId, int setNumber, decimal weight, int reps)
    {
        Id = Guid.NewGuid();
        ExerciseId = exerciseId;
        SetNumber = setNumber;
        Weight = weight;
        Reps = reps;
    }
}
