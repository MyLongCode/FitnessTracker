using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminPanel.ViewModels;

public class UserFoodFormViewModel
{
    public int? Id { get; set; }

    [Required]
    public long UserId { get; set; }

    [Required]
    public int FoodId { get; set; }

    [Range(1, int.MaxValue)]
    public int Weight { get; set; }

    [Required]
    public DateOnly Date { get; set; }

    public IEnumerable<SelectListItem> Users { get; set; } = [];
    public IEnumerable<SelectListItem> Products { get; set; } = [];
}
