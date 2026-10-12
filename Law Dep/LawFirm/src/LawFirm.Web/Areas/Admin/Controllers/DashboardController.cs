using LawFirm.Web.Models;
using LawFirm.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LawFirm.Web.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly IRepository<Appointment> _appointments;
    private readonly IRepository<Lawyer> _lawyers;
    private readonly IRepository<PracticeArea> _areas;

    public DashboardController(IRepository<Appointment> appointments, IRepository<Lawyer> lawyers, IRepository<PracticeArea> areas)
    {
        _appointments = appointments;
        _lawyers = lawyers;
        _areas = areas;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Pending = await _appointments.Query().CountAsync(a => a.Status == AppointmentStatus.Pending);
        ViewBag.Confirmed = await _appointments.Query().CountAsync(a => a.Status == AppointmentStatus.Confirmed);
        ViewBag.Lawyers = await _lawyers.Query().CountAsync();
        ViewBag.Areas = await _areas.Query().CountAsync();

        var upcoming = await _appointments.Query().AsNoTracking()
            .Include(a => a.Lawyer).Include(a => a.PracticeArea)
            .Where(a => a.Status != AppointmentStatus.Cancelled && a.PreferredDate >= DateTime.Today)
            .OrderBy(a => a.PreferredDate)
            .Take(5)
            .ToListAsync();

        return View(upcoming);
    }
}
