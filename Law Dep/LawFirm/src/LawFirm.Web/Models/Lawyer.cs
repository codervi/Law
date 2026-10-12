using System.ComponentModel.DataAnnotations;

namespace LawFirm.Web.Models;

public class Lawyer
{
    public int Id { get; set; }

    [Required, StringLength(100), Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(2000)]
    public string Bio { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(120)]
    public string Email { get; set; } = string.Empty;

    [StringLength(300), Display(Name = "Photo URL")]
    public string? PhotoUrl { get; set; }

    // Many-to-many: EF Core 8 creates the join table automatically.
    public ICollection<PracticeArea> PracticeAreas { get; set; } = new List<PracticeArea>();
}
