using System.ComponentModel.DataAnnotations;
using LawFirm.Web.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LawFirm.Web.ViewModels;

public class AppointmentCreateVm
{
    [Required, StringLength(100), Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(120)]
    public string Email { get; set; } = string.Empty;

    [Phone, StringLength(30)]
    public string? Phone { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Choose a practice area."), Display(Name = "Practice area")]
    public int PracticeAreaId { get; set; }

    [Display(Name = "Attorney")]
    public int? LawyerId { get; set; }

    [Required(ErrorMessage = "Choose a date."), DataType(DataType.Date)]
    public DateTime? Date { get; set; }

    [Required(ErrorMessage = "Choose a time.")]
    public string? Time { get; set; }

    [Required, StringLength(1000, MinimumLength = 10, ErrorMessage = "Please describe your matter in at least 10 characters."),
     Display(Name = "Briefly describe your matter")]
    public string Message { get; set; } = string.Empty;

    [ValidateNever] public List<PracticeArea> Areas { get; set; } = new();
    [ValidateNever] public List<Lawyer> Lawyers { get; set; } = new();
}
