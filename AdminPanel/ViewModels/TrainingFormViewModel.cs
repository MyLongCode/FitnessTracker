using System.ComponentModel.DataAnnotations;

namespace AdminPanel.ViewModels;

public class TrainingFormViewModel
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Duration { get; set; }

    public string? Difficulty { get; set; }
    public string? Inventory { get; set; }
    public string? MuscleGroup { get; set; }
}
