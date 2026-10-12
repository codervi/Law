using System.ComponentModel.DataAnnotations;
using LawFirm.Web.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LawFirm.Web.ViewModels;

public class LawyerFormVm
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

    public List<int> SelectedAreaIds { get; set; } = new();

    [ValidateNever] public List<PracticeArea> AllAreas { get; set; } = new();
}
