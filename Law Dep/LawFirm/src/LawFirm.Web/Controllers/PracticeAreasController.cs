using LawFirm.Web.Models;
using LawFirm.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LawFirm.Web.Controllers;

public class PracticeAreasController : Controller
{
    private readonly IRepository<PracticeArea> _areas;

    public PracticeAreasController(IRepository<PracticeArea> areas) => _areas = areas;

    public async Task<IActionResult> Index()
    {
        var list = await _areas.Query().AsNoTracking()
            .Include(a => a.Lawyers)
            .OrderBy(a => a.Name)
            .ToListAsync();
        return View(list);
    }
}
