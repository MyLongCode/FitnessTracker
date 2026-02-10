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

    public async Task<IActionResult> Details(long id) => (await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == id)) is { } x ? View(x) : NotFound();
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
