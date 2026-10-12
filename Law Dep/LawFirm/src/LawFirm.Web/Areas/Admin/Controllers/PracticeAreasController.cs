using LawFirm.Web.Models;
using LawFirm.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LawFirm.Web.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles = "Admin")]
public class PracticeAreasController : Controller
{
    private readonly IRepository<PracticeArea> _areas;

    public PracticeAreasController(IRepository<PracticeArea> areas) => _areas = areas;

    public async Task<IActionResult> Index()
        => View(await _areas.Query().AsNoTracking().Include(a => a.Lawyers).OrderBy(a => a.Name).ToListAsync());

    [HttpGet]
    public IActionResult Create() => View("Form", new PracticeArea());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description")] PracticeArea area)
    {
        if (!ModelState.IsValid) return View("Form", area);

        await _areas.AddAsync(area);
        TempData["Success"] = $"\"{area.Name}\" was added.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var area = await _areas.GetByIdAsync(id);
        return area is null ? NotFound() : View("Form", area);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] PracticeArea input)
    {
        if (id != input.Id) return BadRequest();
        if (!ModelState.IsValid) return View("Form", input);

        var area = await _areas.GetByIdAsync(id);
        if (area is null) return NotFound();

        area.Name = input.Name;
        area.Description = input.Description;
        await _areas.SaveAsync();

        TempData["Success"] = $"\"{area.Name}\" was updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _areas.DeleteAsync(id);
            TempData["Success"] = "Practice area removed.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "This practice area is used by existing appointments and can't be removed.";
        }
        return RedirectToAction(nameof(Index));
    }
}
