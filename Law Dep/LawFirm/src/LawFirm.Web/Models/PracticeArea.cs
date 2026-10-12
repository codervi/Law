using System.ComponentModel.DataAnnotations;

namespace LawFirm.Web.Models;

public class PracticeArea
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public ICollection<Lawyer> Lawyers { get; set; } = new List<Lawyer>();
}
