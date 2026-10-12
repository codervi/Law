using LawFirm.Web.Models;

namespace LawFirm.Web.ViewModels;

public class HomeVm
{
    public List<PracticeArea> Areas { get; set; } = new();
    public List<Lawyer> Lawyers { get; set; } = new();
}
