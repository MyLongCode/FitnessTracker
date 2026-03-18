using AdminPanel.Models;

namespace AdminPanel.ViewModels;

public class UserDetailsViewModel
{
    public required User User { get; init; }
    public int TotalWorkouts { get; init; }
    public int DaysRegistered { get; init; }
    public DateOnly? FirstActivityDate { get; init; }
    public IReadOnlyList<DailyMealStatViewModel> MealsLast7Days { get; init; } = [];
}

public class DailyMealStatViewModel
{
    public required string Label { get; init; }
    public DateOnly Date { get; init; }
    public int MealsCount { get; init; }
}
