namespace AdminPanel.Models;

public class Training
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string? Difficulty { get; set; }
    public string? Inventory { get; set; }
    public string? MuscleGroup { get; set; }

    public ICollection<TrainsExercise> TrainsExercises { get; set; } = new List<TrainsExercise>();
    public ICollection<UsersTrain> UsersTrain { get; set; } = new List<UsersTrain>();
}
