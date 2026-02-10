using AdminPanel.Data;
using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Controllers;

[Authorize]
public class TrainingsController(AppDbContext context) : Controller
{
    public async Task<IActionResult> Index(string? search, string sort = "id", int page = 1, int pageSize = 10)
    {
        var query = context.Trainings.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.Name.Contains(search));
        query = sort switch
        {
            "name" => query.OrderBy(x => x.Name),
            "name_desc" => query.OrderByDescending(x => x.Name),
            "id_desc" => query.OrderByDescending(x => x.Id),
            _ => query.OrderBy(x => x.Id)
        };
        ViewBag.Search = search;
        return View(await query.ToPagedResultAsync(page, pageSize));
    }

    public async Task<IActionResult> Details(int id) => (await context.Trainings.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id)) is { } x ? View(x) : NotFound();
    public IActionResult Create() => View(new TrainingFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TrainingFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        context.Trainings.Add(new Training { Id = model.Id, Name = model.Name, Duration = model.Duration, Difficulty = model.Difficulty, Inventory = model.Inventory, MuscleGroup = model.MuscleGroup });
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var x = await context.Trainings.FindAsync(id);
        if (x == null) return NotFound();
        return View(new TrainingFormViewModel { Id = x.Id, Name = x.Name, Duration = x.Duration, Difficulty = x.Difficulty, Inventory = x.Inventory, MuscleGroup = x.MuscleGroup });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TrainingFormViewModel model)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid) return View(model);
        var x = await context.Trainings.FindAsync(id);
        if (x == null) return NotFound();
        x.Name = model.Name; x.Duration = model.Duration; x.Difficulty = model.Difficulty; x.Inventory = model.Inventory; x.MuscleGroup = model.MuscleGroup;
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id) => (await context.Trainings.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id)) is { } x ? View(x) : NotFound();

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var x = await context.Trainings.FindAsync(id);
        if (x == null) return RedirectToAction(nameof(Index));
        context.Trainings.Remove(x);
        try { await context.SaveChangesAsync(); }
        catch (DbUpdateException) { TempData["Error"] = "Нельзя удалить: есть связанные записи."; return RedirectToAction(nameof(Delete), new { id }); }
        return RedirectToAction(nameof(Index));
    }
}
