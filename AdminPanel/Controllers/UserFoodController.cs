using AdminPanel.Data;
using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Controllers;

public class UserFoodController(AppDbContext context) : Controller
{
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var query = context.UserFood.AsNoTracking().Include(x => x.User).Include(x => x.Food).OrderByDescending(x => x.Date).AsQueryable();
        return View(await query.ToPagedResultAsync(page, pageSize));
    }

    public async Task<IActionResult> Details(int id)
    {
        var x = await context.UserFood.AsNoTracking().Include(x => x.User).Include(x => x.Food).FirstOrDefaultAsync(x => x.Id == id);
        return x == null ? NotFound() : View(x);
    }

    public async Task<IActionResult> Create() => View(await BuildVm());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserFoodFormViewModel model)
    {
        if (!ModelState.IsValid) return View(await BuildVm(model));
        context.UserFood.Add(new UserFood { UserId = model.UserId, FoodId = model.FoodId, Weight = model.Weight, Date = model.Date });
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var x = await context.UserFood.FindAsync(id);
        if (x == null) return NotFound();
        return View(await BuildVm(new UserFoodFormViewModel { Id = x.Id, UserId = x.UserId, FoodId = x.FoodId, Weight = x.Weight, Date = x.Date }));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UserFoodFormViewModel model)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid) return View(await BuildVm(model));
        var x = await context.UserFood.FindAsync(id);
        if (x == null) return NotFound();
        x.UserId = model.UserId; x.FoodId = model.FoodId; x.Weight = model.Weight; x.Date = model.Date;
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var x = await context.UserFood.AsNoTracking().Include(x => x.User).Include(x => x.Food).FirstOrDefaultAsync(x => x.Id == id);
        return x == null ? NotFound() : View(x);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var x = await context.UserFood.FindAsync(id);
        if (x == null) return RedirectToAction(nameof(Index));
        context.UserFood.Remove(x);
        try { await context.SaveChangesAsync(); }
        catch (DbUpdateException) { TempData["Error"] = "Нельзя удалить: есть связанные записи."; return RedirectToAction(nameof(Delete), new { id }); }
        return RedirectToAction(nameof(Index));
    }

    private async Task<UserFoodFormViewModel> BuildVm(UserFoodFormViewModel? vm = null)
    {
        vm ??= new UserFoodFormViewModel { Date = DateOnly.FromDateTime(DateTime.UtcNow) };
        vm.Users = (await context.Users.AsNoTracking().OrderBy(x => x.Name).ToListAsync()).Select(x => new SelectListItem($"{x.UserId} - {x.Name}", x.UserId.ToString()));
        vm.Products = (await context.Products.AsNoTracking().OrderBy(x => x.Name).ToListAsync()).Select(x => new SelectListItem($"{x.Id} - {x.Name}", x.Id.ToString()));
        return vm;
    }
}
