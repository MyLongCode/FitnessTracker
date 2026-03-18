using AdminPanel.Data;
using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Controllers;

public class UsersController(AppDbContext context) : Controller
{
    public async Task<IActionResult> Index(string? search, string sort = "id", int page = 1, int pageSize = 10)
    {
        var query = context.Users.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.Name != null && x.Name.Contains(search));
        query = sort switch
        {
            "name" => query.OrderBy(x => x.Name),
            "name_desc" => query.OrderByDescending(x => x.Name),
            "id_desc" => query.OrderByDescending(x => x.UserId),
            _ => query.OrderBy(x => x.UserId)
        };
        ViewBag.Search = search;
        return View(await query.ToPagedResultAsync(page, pageSize));
    }

    public async Task<IActionResult> Details(long id)
    {
        var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == id);
        if (user == null) return NotFound();

        var totalWorkoutsTask = context.UsersTrain
            .AsNoTracking()
            .Where(x => x.UserId == id)
            .CountAsync();

        var earliestWorkoutTask = context.UsersTrain
            .AsNoTracking()
            .Where(x => x.UserId == id)
            .Select(x => (DateOnly?)x.DateCreated)
            .MinAsync();

        var earliestMealTask = context.UserFood
            .AsNoTracking()
            .Where(x => x.UserId == id)
            .Select(x => (DateOnly?)x.Date)
            .MinAsync();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var periodStart = today.AddDays(-6);

        var mealStatsTask = context.UserFood
            .AsNoTracking()
            .Where(x => x.UserId == id && x.Date >= periodStart && x.Date <= today)
            .GroupBy(x => x.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync();

        await Task.WhenAll(totalWorkoutsTask, earliestWorkoutTask, earliestMealTask, mealStatsTask);

        var firstActivityDate = new[] { earliestWorkoutTask.Result, earliestMealTask.Result }
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .DefaultIfEmpty()
            .Min();

        var hasActivity = earliestWorkoutTask.Result.HasValue || earliestMealTask.Result.HasValue;
        var mealsByDate = mealStatsTask.Result.ToDictionary(x => x.Date, x => x.Count);
        var mealsLast7Days = Enumerable.Range(0, 7)
            .Select(offset => periodStart.AddDays(offset))
            .Select(date => new DailyMealStatViewModel
            {
                Date = date,
                Label = date.ToString("dd.MM"),
                MealsCount = mealsByDate.GetValueOrDefault(date)
            })
            .ToList();

        var model = new UserDetailsViewModel
        {
            User = user,
            TotalWorkouts = totalWorkoutsTask.Result,
            DaysRegistered = hasActivity ? today.DayNumber - firstActivityDate.DayNumber + 1 : 0,
            FirstActivityDate = hasActivity ? firstActivityDate : null,
            MealsLast7Days = mealsLast7Days
        };

        return View(model);
    }

    public IActionResult Create() => View(new UserFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        context.Users.Add(new User { UserId = model.UserId, Name = model.Name, Weight = model.Weight, Height = model.Height, FoodLike = model.FoodLike });
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(long id)
    {
        var x = await context.Users.FindAsync(id);
        if (x == null) return NotFound();
        return View(new UserFormViewModel { UserId = x.UserId, Name = x.Name, Weight = x.Weight, Height = x.Height, FoodLike = x.FoodLike });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, UserFormViewModel model)
    {
        if (id != model.UserId) return NotFound();
        if (!ModelState.IsValid) return View(model);
        var x = await context.Users.FindAsync(id);
        if (x == null) return NotFound();
        x.Name = model.Name; x.Weight = model.Weight; x.Height = model.Height; x.FoodLike = model.FoodLike;
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(long id) => (await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == id)) is { } x ? View(x) : NotFound();

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        var x = await context.Users.FindAsync(id);
        if (x == null) return RedirectToAction(nameof(Index));
        context.Users.Remove(x);
        try { await context.SaveChangesAsync(); }
        catch (DbUpdateException) { TempData["Error"] = "Нельзя удалить: есть связанные записи."; return RedirectToAction(nameof(Delete), new { id }); }
        return RedirectToAction(nameof(Index));
    }
}
