using System.ComponentModel.DataAnnotations;

namespace AdminPanel.ViewModels;

public class ReccomendationFormViewModel
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? Image { get; set; }
}
