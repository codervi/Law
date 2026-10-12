using LawFirm.Web.Models;
using LawFirm.Web.Services;
using LawFirm.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LawFirm.Web.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles = "Admin")]
public class AppointmentsController : Controller
{
    private const int PageSize = 10;
    private readonly IRepository<Appointment> _appointments;

    public AppointmentsController(IRepository<Appointment> appointments) => _appointments = appointments;

    public async Task<IActionResult> Index(AppointmentStatus? status, int page = 1)
    {
        var query = _appointments.Query().AsNoTracking()
            .Include(a => a.Lawyer).Include(a => a.PracticeArea)
            .AsQueryable();

        if (status.HasValue) query = query.Where(a => a.Status == status);

        var total = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var items = await query
            .OrderByDescending(a => a.PreferredDate)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        return View(new PagedVm<Appointment>
        {
            Items = items,
            Page = page,
            TotalPages = totalPages,
            Status = status?.ToString()
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SetStatus(int id, AppointmentStatus status, string? returnStatus, int page = 1)
    {
        var appointment = await _appointments.GetByIdAsync(id);
        if (appointment is null) return NotFound();

        appointment.Status = status;
        try
        {
            await _appointments.SaveAsync();
            TempData["Success"] = $"Appointment marked as {status.ToString().ToLower()}.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Can't confirm: this attorney already has an active appointment in that slot.";
        }

        return RedirectToAction(nameof(Index), new { status = returnStatus, page });
    }
}
