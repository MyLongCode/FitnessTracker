namespace AdminPanel.Models;

public class TrainsExercise
{
    public int TrainId { get; set; }
    public int ExerciseId { get; set; }
    public int Ammount { get; set; }

    public Training? Train { get; set; }
    public Exercise? Exercise { get; set; }
}
