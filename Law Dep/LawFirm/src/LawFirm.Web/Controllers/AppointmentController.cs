using LawFirm.Web.Models;
using LawFirm.Web.Services;
using LawFirm.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LawFirm.Web.Controllers;

public class AppointmentController : Controller
{
    private readonly IAppointmentService _service;
    private readonly IRepository<PracticeArea> _areas;
    private readonly IRepository<Lawyer> _lawyers;

    public AppointmentController(IAppointmentService service, IRepository<PracticeArea> areas, IRepository<Lawyer> lawyers)
    {
        _service = service;
        _areas = areas;
        _lawyers = lawyers;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? lawyerId)
    {
        var vm = new AppointmentCreateVm { LawyerId = lawyerId };
        await FillListsAsync(vm);
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AppointmentCreateVm vm)
    {
        if (ModelState.IsValid)
        {
            var result = await _service.CreateAsync(vm);
            if (result.Success) return RedirectToAction(nameof(Success));
            ModelState.AddModelError(string.Empty, result.Error ?? "Something went wrong.");
        }

        await FillListsAsync(vm);
        return View(vm);
    }

    public IActionResult Success() => View();

    // Called by wwwroot/js/site.js with fetch() to fill the time dropdown.
    [HttpGet]
    public async Task<IActionResult> Slots(int? lawyerId, DateTime date)
        => Json(await _service.GetAvailableSlotsAsync(lawyerId, date));

    private async Task FillListsAsync(AppointmentCreateVm vm)
    {
        vm.Areas = await _areas.Query().AsNoTracking().OrderBy(a => a.Name).ToListAsync();
        vm.Lawyers = await _lawyers.Query().AsNoTracking().OrderBy(l => l.FullName).ToListAsync();
    }
}
