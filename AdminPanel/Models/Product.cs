namespace AdminPanel.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Image { get; set; }
    public int Kcal { get; set; }
    public int Protein { get; set; }
    public int Carbohyd { get; set; }
    public int Fat { get; set; }

    public ICollection<UserFood> UserFood { get; set; } = new List<UserFood>();
}
