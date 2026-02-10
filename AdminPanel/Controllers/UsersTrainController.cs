using AdminPanel.Data;
using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Controllers;

[Authorize]
public class UsersTrainController(AppDbContext context) : Controller
{
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var query = context.UsersTrain.AsNoTracking().Include(x => x.User).Include(x => x.Train)
            .OrderByDescending(x => x.DateCreated).AsQueryable();
        return View(await query.ToPagedResultAsync(page, pageSize));
    }

    public async Task<IActionResult> Details(long userId, int trainId, DateOnly dateCreated)
    {
        var x = await context.UsersTrain.AsNoTracking().Include(x => x.User).Include(x => x.Train)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.TrainId == trainId && x.DateCreated == dateCreated);
        return x == null ? NotFound() : View(x);
    }

    public async Task<IActionResult> Create() => View(await BuildVm());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsersTrainFormViewModel model)
    {
        if (!ModelState.IsValid) return View(await BuildVm(model));
        context.UsersTrain.Add(new UsersTrain { UserId = model.UserId, TrainId = model.TrainId, DateCreated = model.DateCreated });
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(long userId, int trainId, DateOnly dateCreated)
    {
        var x = await context.UsersTrain.FindAsync(userId, trainId, dateCreated);
        if (x == null) return NotFound();
        return View(await BuildVm(new UsersTrainFormViewModel { UserId = x.UserId, TrainId = x.TrainId, DateCreated = x.DateCreated }));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long userId, int trainId, DateOnly dateCreated, UsersTrainFormViewModel model)
    {
        if (userId != model.UserId || trainId != model.TrainId || dateCreated != model.DateCreated) return NotFound();
        if (!ModelState.IsValid) return View(await BuildVm(model));
        var x = await context.UsersTrain.FindAsync(userId, trainId, dateCreated);
        if (x == null) return NotFound();
        x.UserId = model.UserId; x.TrainId = model.TrainId; x.DateCreated = model.DateCreated;
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(long userId, int trainId, DateOnly dateCreated)
    {
        var x = await context.UsersTrain.AsNoTracking().Include(x => x.User).Include(x => x.Train)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.TrainId == trainId && x.DateCreated == dateCreated);
        return x == null ? NotFound() : View(x);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long userId, int trainId, DateOnly dateCreated)
    {
        var x = await context.UsersTrain.FindAsync(userId, trainId, dateCreated);
        if (x == null) return RedirectToAction(nameof(Index));
        context.UsersTrain.Remove(x);
        try { await context.SaveChangesAsync(); }
        catch (DbUpdateException) { TempData["Error"] = "Нельзя удалить: есть связанные записи."; return RedirectToAction(nameof(Delete), new { userId, trainId, dateCreated }); }
        return RedirectToAction(nameof(Index));
    }

    private async Task<UsersTrainFormViewModel> BuildVm(UsersTrainFormViewModel? vm = null)
    {
        vm ??= new UsersTrainFormViewModel { DateCreated = DateOnly.FromDateTime(DateTime.UtcNow) };
        vm.Users = (await context.Users.AsNoTracking().OrderBy(x => x.Name).ToListAsync()).Select(x => new SelectListItem($"{x.UserId} - {x.Name}", x.UserId.ToString()));
        vm.Trainings = (await context.Trainings.AsNoTracking().OrderBy(x => x.Name).ToListAsync()).Select(x => new SelectListItem($"{x.Id} - {x.Name}", x.Id.ToString()));
        return vm;
    }
}
