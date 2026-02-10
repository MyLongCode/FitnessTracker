namespace AdminPanel.Models;

public class UserFood
{
    public int Id { get; set; }
    public long UserId { get; set; }
    public int FoodId { get; set; }
    public int Weight { get; set; }
    public DateOnly Date { get; set; }

    public User? User { get; set; }
    public Product? Food { get; set; }
}
