using LawFirm.Web.Models;
using LawFirm.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LawFirm.Web.Controllers;

public class LawyersController : Controller
{
    private readonly IRepository<Lawyer> _lawyers;

    public LawyersController(IRepository<Lawyer> lawyers) => _lawyers = lawyers;

    public async Task<IActionResult> Index()
    {
        var list = await _lawyers.Query().AsNoTracking()
            .Include(l => l.PracticeAreas)
            .OrderBy(l => l.Id)
            .ToListAsync();
        return View(list);
    }

    public async Task<IActionResult> Details(int id)
    {
        var lawyer = await _lawyers.Query().AsNoTracking()
            .Include(l => l.PracticeAreas)
            .FirstOrDefaultAsync(l => l.Id == id);

        return lawyer is null ? NotFound() : View(lawyer);
    }
}
