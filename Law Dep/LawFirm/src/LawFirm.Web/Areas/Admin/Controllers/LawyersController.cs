using LawFirm.Web.Models;
using LawFirm.Web.Services;
using LawFirm.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LawFirm.Web.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles = "Admin")]
public class LawyersController : Controller
{
    private readonly IRepository<Lawyer> _lawyers;
    private readonly IRepository<PracticeArea> _areas;

    public LawyersController(IRepository<Lawyer> lawyers, IRepository<PracticeArea> areas)
    {
        _lawyers = lawyers;
        _areas = areas;
    }

    public async Task<IActionResult> Index()
        => View(await _lawyers.Query().AsNoTracking().Include(l => l.PracticeAreas).OrderBy(l => l.FullName).ToListAsync());

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new LawyerFormVm();
        await LoadAreasAsync(vm);
        return View("Form", vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LawyerFormVm vm)
    {
        if (!ModelState.IsValid)
        {
            await LoadAreasAsync(vm);
            return View("Form", vm);
        }

        var lawyer = new Lawyer
        {
            FullName = vm.FullName,
            Title = vm.Title,
            Bio = vm.Bio,
            Email = vm.Email,
            PhotoUrl = string.IsNullOrWhiteSpace(vm.PhotoUrl) ? null : vm.PhotoUrl,
            PracticeAreas = await _areas.Query().Where(a => vm.SelectedAreaIds.Contains(a.Id)).ToListAsync()
        };

        await _lawyers.AddAsync(lawyer);
        TempData["Success"] = $"{lawyer.FullName} was added.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var lawyer = await _lawyers.Query().AsNoTracking().Include(l => l.PracticeAreas).FirstOrDefaultAsync(l => l.Id == id);
        if (lawyer is null) return NotFound();

        var vm = new LawyerFormVm
        {
            Id = lawyer.Id,
            FullName = lawyer.FullName,
            Title = lawyer.Title,
            Bio = lawyer.Bio,
            Email = lawyer.Email,
            PhotoUrl = lawyer.PhotoUrl,
            SelectedAreaIds = lawyer.PracticeAreas.Select(a => a.Id).ToList()
        };
        await LoadAreasAsync(vm);
        return View("Form", vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LawyerFormVm vm)
    {
        if (id != vm.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            await LoadAreasAsync(vm);
            return View("Form", vm);
        }

        var lawyer = await _lawyers.Query().Include(l => l.PracticeAreas).FirstOrDefaultAsync(l => l.Id == id);
        if (lawyer is null) return NotFound();

        lawyer.FullName = vm.FullName;
        lawyer.Title = vm.Title;
        lawyer.Bio = vm.Bio;
        lawyer.Email = vm.Email;
        lawyer.PhotoUrl = string.IsNullOrWhiteSpace(vm.PhotoUrl) ? null : vm.PhotoUrl;

        lawyer.PracticeAreas.Clear();
        foreach (var area in await _areas.Query().Where(a => vm.SelectedAreaIds.Contains(a.Id)).ToListAsync())
            lawyer.PracticeAreas.Add(area);

        await _lawyers.SaveAsync();
        TempData["Success"] = $"{lawyer.FullName} was updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _lawyers.DeleteAsync(id);
        TempData["Success"] = "Attorney removed. Their past appointments are kept without an assigned attorney.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadAreasAsync(LawyerFormVm vm)
        => vm.AllAreas = await _areas.Query().AsNoTracking().OrderBy(a => a.Name).ToListAsync();
}
