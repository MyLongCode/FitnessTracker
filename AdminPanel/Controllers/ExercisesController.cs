using AdminPanel.Data;
using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Controllers;

[Authorize]
public class ExercisesController(AppDbContext context) : Controller
{
    public async Task<IActionResult> Index(string? search, string sort = "id", int page = 1, int pageSize = 10)
    {
        var query = context.Exercises.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.Name != null && x.Name.Contains(search));

        query = sort switch
        {
            "name" => query.OrderBy(x => x.Name),
            "name_desc" => query.OrderByDescending(x => x.Name),
            "id_desc" => query.OrderByDescending(x => x.Id),
            _ => query.OrderBy(x => x.Id)
        };

        ViewBag.Search = search;
        ViewBag.Sort = sort;
        var result = await query.ToPagedResultAsync(page, pageSize);
        return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await context.Exercises.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return item == null ? NotFound() : View(item);
    }

    public IActionResult Create() => View(new ExerciseFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExerciseFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        context.Exercises.Add(new Exercise { Id = model.Id, Name = model.Name, MuscleGroup = model.MuscleGroup });
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await context.Exercises.FindAsync(id);
        if (item == null) return NotFound();
        return View(new ExerciseFormViewModel { Id = item.Id, Name = item.Name ?? string.Empty, MuscleGroup = item.MuscleGroup });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ExerciseFormViewModel model)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid) return View(model);
        var item = await context.Exercises.FindAsync(id);
        if (item == null) return NotFound();
        item.Name = model.Name;
        item.MuscleGroup = model.MuscleGroup;
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var item = await context.Exercises.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await context.Exercises.FindAsync(id);
        if (item == null) return RedirectToAction(nameof(Index));
        context.Exercises.Remove(item);
        try { await context.SaveChangesAsync(); }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Нельзя удалить: есть связанные записи.";
            return RedirectToAction(nameof(Delete), new { id });
        }
        return RedirectToAction(nameof(Index));
    }
}
