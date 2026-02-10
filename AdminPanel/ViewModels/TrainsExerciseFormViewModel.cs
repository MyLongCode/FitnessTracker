using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminPanel.ViewModels;

public class TrainsExerciseFormViewModel
{
    [Required]
    public int TrainId { get; set; }

    [Required]
    public int ExerciseId { get; set; }

    [Range(1, int.MaxValue)]
    public int Ammount { get; set; }

    public IEnumerable<SelectListItem> Trainings { get; set; } = [];
    public IEnumerable<SelectListItem> Exercises { get; set; } = [];
}
