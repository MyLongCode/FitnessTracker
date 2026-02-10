using AdminPanel.Data;
using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Controllers;

public class ReccomendationsController(AppDbContext context) : Controller
{
    public async Task<IActionResult> Index(string? search, string sort = "id", int page = 1, int pageSize = 10)
    {
        var query = context.Reccomendations.AsNoTracking();
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

    public async Task<IActionResult> Details(int id) => (await context.Reccomendations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id)) is { } x ? View(x) : NotFound();
    public IActionResult Create() => View(new ReccomendationFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReccomendationFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        context.Reccomendations.Add(new Reccomendation { Id = model.Id, Name = model.Name, Description = model.Description, Image = model.Image });
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var x = await context.Reccomendations.FindAsync(id);
        if (x == null) return NotFound();
        return View(new ReccomendationFormViewModel { Id = x.Id, Name = x.Name, Description = x.Description, Image = x.Image });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ReccomendationFormViewModel model)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid) return View(model);
        var x = await context.Reccomendations.FindAsync(id);
        if (x == null) return NotFound();
        x.Name = model.Name; x.Description = model.Description; x.Image = model.Image;
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id) => (await context.Reccomendations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id)) is { } x ? View(x) : NotFound();

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var x = await context.Reccomendations.FindAsync(id);
        if (x == null) return RedirectToAction(nameof(Index));
        context.Reccomendations.Remove(x);
        try { await context.SaveChangesAsync(); }
        catch (DbUpdateException) { TempData["Error"] = "Нельзя удалить: есть связанные записи."; return RedirectToAction(nameof(Delete), new { id }); }
        return RedirectToAction(nameof(Index));
    }
}
