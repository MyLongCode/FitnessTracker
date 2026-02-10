namespace AdminPanel.Models;

public class Exercise
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? MuscleGroup { get; set; }

    public ICollection<TrainsExercise> TrainsExercises { get; set; } = new List<TrainsExercise>();
}
