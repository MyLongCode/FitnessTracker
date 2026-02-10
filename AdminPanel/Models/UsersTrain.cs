namespace AdminPanel.Models;

public class UsersTrain
{
    public long UserId { get; set; }
    public int TrainId { get; set; }
    public DateOnly DateCreated { get; set; }

    public User? User { get; set; }
    public Training? Train { get; set; }
}
