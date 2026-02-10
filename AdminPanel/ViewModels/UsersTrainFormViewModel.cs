using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminPanel.ViewModels;

public class UsersTrainFormViewModel
{
    [Required]
    public long UserId { get; set; }

    [Required]
    public int TrainId { get; set; }

    [Required]
    public DateOnly DateCreated { get; set; }

    public IEnumerable<SelectListItem> Users { get; set; } = [];
    public IEnumerable<SelectListItem> Trainings { get; set; } = [];
}
