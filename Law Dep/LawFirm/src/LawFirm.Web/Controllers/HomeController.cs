using LawFirm.Web.Models;
using LawFirm.Web.Services;
using LawFirm.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LawFirm.Web.Controllers;

public class HomeController : Controller
{
    private readonly IRepository<PracticeArea> _areas;
    private readonly IRepository<Lawyer> _lawyers;

    public HomeController(IRepository<PracticeArea> areas, IRepository<Lawyer> lawyers)
    {
        _areas = areas;
        _lawyers = lawyers;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new HomeVm
        {
            Areas = await _areas.Query().AsNoTracking().OrderBy(a => a.Name).ToListAsync(),
            Lawyers = await _lawyers.Query().AsNoTracking().OrderBy(l => l.Id).Take(4).ToListAsync()
        };
        return View(vm);
    }

    public IActionResult Error() => View();
}
