using AdminPanel.Data;
using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Controllers;

[Authorize]
public class ProductsController(AppDbContext context) : Controller
{
    public async Task<IActionResult> Index(string? search, string sort = "id", int page = 1, int pageSize = 10)
    {
        var query = context.Products.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.Name.Contains(search));
        query = sort switch
        {
            "name" => query.OrderBy(x => x.Name),
            "name_desc" => query.OrderByDescending(x => x.Name),
            "id_desc" => query.OrderByDescending(x => x.Id),
            _ => query.OrderBy(x => x.Id)
        };
        ViewBag.Search = search; ViewBag.Sort = sort;
        return View(await query.ToPagedResultAsync(page, pageSize));
    }

    public async Task<IActionResult> Details(int id) => (await context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id)) is { } x ? View(x) : NotFound();
    public IActionResult Create() => View(new ProductFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        context.Products.Add(new Product { Id = model.Id, Name = model.Name, Image = model.Image, Kcal = model.Kcal, Protein = model.Protein, Carbohyd = model.Carbohyd, Fat = model.Fat });
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var x = await context.Products.FindAsync(id);
        if (x == null) return NotFound();
        return View(new ProductFormViewModel { Id = x.Id, Name = x.Name, Image = x.Image, Kcal = x.Kcal, Protein = x.Protein, Carbohyd = x.Carbohyd, Fat = x.Fat });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductFormViewModel model)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid) return View(model);
        var x = await context.Products.FindAsync(id);
        if (x == null) return NotFound();
        x.Name = model.Name; x.Image = model.Image; x.Kcal = model.Kcal; x.Protein = model.Protein; x.Carbohyd = model.Carbohyd; x.Fat = model.Fat;
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id) => (await context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id)) is { } x ? View(x) : NotFound();

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var x = await context.Products.FindAsync(id);
        if (x == null) return RedirectToAction(nameof(Index));
        context.Products.Remove(x);
        try { await context.SaveChangesAsync(); }
        catch (DbUpdateException) { TempData["Error"] = "Нельзя удалить: есть связанные записи."; return RedirectToAction(nameof(Delete), new { id }); }
        return RedirectToAction(nameof(Index));
    }
}
