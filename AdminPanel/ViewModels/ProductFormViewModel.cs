using System.ComponentModel.DataAnnotations;

namespace AdminPanel.ViewModels;

public class ProductFormViewModel
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Image { get; set; }

    [Range(0, int.MaxValue)]
    public int Kcal { get; set; }

    [Range(0, int.MaxValue)]
    public int Protein { get; set; }

    [Range(0, int.MaxValue)]
    public int Carbohyd { get; set; }

    [Range(0, int.MaxValue)]
    public int Fat { get; set; }
}
