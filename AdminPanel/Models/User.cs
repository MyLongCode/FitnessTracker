namespace AdminPanel.Models;

public class User
{
    public long UserId { get; set; }
    public string? Name { get; set; }
    public int Weight { get; set; }
    public int Height { get; set; }
    public string? FoodLike { get; set; }

    public ICollection<UserFood> UserFood { get; set; } = new List<UserFood>();
    public ICollection<UsersTrain> UsersTrain { get; set; } = new List<UsersTrain>();
}
