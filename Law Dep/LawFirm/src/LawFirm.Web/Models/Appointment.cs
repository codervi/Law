using System.ComponentModel.DataAnnotations;

namespace LawFirm.Web.Models;

public enum AppointmentStatus
{
    Pending,
    Confirmed,
    Cancelled
}

public class Appointment
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(120)]
    public string Email { get; set; } = string.Empty;

    [StringLength(30)]
    public string? Phone { get; set; }

    public int PracticeAreaId { get; set; }
    public PracticeArea? PracticeArea { get; set; }

    public int? LawyerId { get; set; }
    public Lawyer? Lawyer { get; set; }

    public DateTime PreferredDate { get; set; }

    [Required, StringLength(1000)]
    public string Message { get; set; } = string.Empty;

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
