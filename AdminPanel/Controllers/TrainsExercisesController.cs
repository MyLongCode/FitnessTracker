using AdminPanel.Data;
using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Controllers;

public class TrainsExercisesController(AppDbContext context) : Controller
{
    public async Task<IActionResult> Index(int? trainId, int page = 1, int pageSize = 10)
    {
        var query = context.TrainsExercises.AsNoTracking().Include(x => x.Train).Include(x => x.Exercise).AsQueryable();
        if (trainId.HasValue) query = query.Where(x => x.TrainId == trainId.Value);

        ViewBag.TrainId = new SelectList(await context.Trainings.AsNoTracking().OrderBy(x => x.Name).ToListAsync(), "Id", "Name", trainId);
        return View(await query.OrderBy(x => x.TrainId).ThenBy(x => x.ExerciseId).ToPagedResultAsync(page, pageSize));
    }

    public async Task<IActionResult> Details(int trainId, int exerciseId)
    {
        var item = await context.TrainsExercises.AsNoTracking().Include(x => x.Train).Include(x => x.Exercise)
            .FirstOrDefaultAsync(x => x.TrainId == trainId && x.ExerciseId == exerciseId);
        return item == null ? NotFound() : View(item);
    }

    public async Task<IActionResult> Create()
    {
        return View(await BuildVm());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TrainsExerciseFormViewModel model)
    {
        if (!ModelState.IsValid) return View(await BuildVm(model));
        context.TrainsExercises.Add(new TrainsExercise { TrainId = model.TrainId, ExerciseId = model.ExerciseId, Ammount = model.Ammount });
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int trainId, int exerciseId)
    {
        var x = await context.TrainsExercises.FindAsync(trainId, exerciseId);
        if (x == null) return NotFound();
        return View(await BuildVm(new TrainsExerciseFormViewModel { TrainId = x.TrainId, ExerciseId = x.ExerciseId, Ammount = x.Ammount }));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int trainId, int exerciseId, TrainsExerciseFormViewModel model)
    {
        if (trainId != model.TrainId || exerciseId != model.ExerciseId) return NotFound();
        if (!ModelState.IsValid) return View(await BuildVm(model));
        var x = await context.TrainsExercises.FindAsync(trainId, exerciseId);
        if (x == null) return NotFound();
        x.Ammount = model.Ammount;
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int trainId, int exerciseId)
    {
        var item = await context.TrainsExercises.AsNoTracking().Include(x => x.Train).Include(x => x.Exercise)
            .FirstOrDefaultAsync(x => x.TrainId == trainId && x.ExerciseId == exerciseId);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int trainId, int exerciseId)
    {
        var x = await context.TrainsExercises.FindAsync(trainId, exerciseId);
        if (x == null) return RedirectToAction(nameof(Index));
        context.TrainsExercises.Remove(x);
        try { await context.SaveChangesAsync(); }
        catch (DbUpdateException) { TempData["Error"] = "Нельзя удалить: есть связанные записи."; return RedirectToAction(nameof(Delete), new { trainId, exerciseId }); }
        return RedirectToAction(nameof(Index));
    }

    private async Task<TrainsExerciseFormViewModel> BuildVm(TrainsExerciseFormViewModel? vm = null)
    {
        vm ??= new TrainsExerciseFormViewModel();
        vm.Trainings = (await context.Trainings.AsNoTracking().OrderBy(x => x.Name).ToListAsync()).Select(x => new SelectListItem($"{x.Id} - {x.Name}", x.Id.ToString()));
        vm.Exercises = (await context.Exercises.AsNoTracking().OrderBy(x => x.Name).ToListAsync()).Select(x => new SelectListItem($"{x.Id} - {x.Name}", x.Id.ToString()));
        return vm;
    }
}
