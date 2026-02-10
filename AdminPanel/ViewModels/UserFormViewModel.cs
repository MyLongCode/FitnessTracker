using System.ComponentModel.DataAnnotations;

namespace AdminPanel.ViewModels;

public class UserFormViewModel
{
    [Required]
    public long UserId { get; set; }

    public string? Name { get; set; }

    [Range(0, int.MaxValue)]
    public int Weight { get; set; }

    [Range(0, int.MaxValue)]
    public int Height { get; set; }

    public string? FoodLike { get; set; }
}
