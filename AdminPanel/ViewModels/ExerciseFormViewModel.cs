using System.ComponentModel.DataAnnotations;

namespace AdminPanel.ViewModels;

public class ExerciseFormViewModel
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? MuscleGroup { get; set; }
}
