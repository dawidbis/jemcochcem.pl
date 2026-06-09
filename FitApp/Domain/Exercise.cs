namespace FitApp.Domain.Entities;

public class Exercise
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string MuscleGroup { get; set; } = string.Empty;

   public bool IsCustom { get; set; }

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
