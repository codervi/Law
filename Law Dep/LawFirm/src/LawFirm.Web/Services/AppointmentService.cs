using LawFirm.Web.Data;
using LawFirm.Web.Models;
using LawFirm.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LawFirm.Web.Services;

/// <summary>
/// Business rules for booking consultations:
/// weekdays only, hourly slots from 09:00 to 16:00, no past times,
/// and a lawyer cannot be booked twice for the same slot.
/// </summary>
public class AppointmentService : IAppointmentService
{
    private static readonly TimeOnly[] DailySlots =
        Enumerable.Range(9, 8).Select(h => new TimeOnly(h, 0)).ToArray();

    private readonly AppDbContext _db;

    public AppointmentService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<string>> GetAvailableSlotsAsync(int? lawyerId, DateTime date)
    {
        date = date.Date;

        if (date < DateTime.Today || date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            return Array.Empty<string>();

        var taken = new HashSet<TimeOnly>();
        if (lawyerId.HasValue)
        {
            var booked = await _db.Appointments
                .Where(a => a.LawyerId == lawyerId
                            && a.Status != AppointmentStatus.Cancelled
                            && a.PreferredDate >= date
                            && a.PreferredDate < date.AddDays(1))
                .Select(a => a.PreferredDate)
                .ToListAsync();

            taken = booked.Select(TimeOnly.FromDateTime).ToHashSet();
        }

        var now = DateTime.Now;
        return DailySlots
            .Where(s => !taken.Contains(s) && date.Add(s.ToTimeSpan()) > now)
            .Select(s => s.ToString("HH:mm"))
            .ToList();
    }

    public async Task<AppointmentResult> CreateAsync(AppointmentCreateVm vm)
    {
        if (vm.Date is null || !TimeOnly.TryParseExact(vm.Time, "HH:mm", out var time))
            return new AppointmentResult(false, "Choose a valid date and time.");

        var slots = await GetAvailableSlotsAsync(vm.LawyerId, vm.Date.Value);
        if (!slots.Contains(time.ToString("HH:mm")))
            return new AppointmentResult(false, "That time is no longer available. Please pick another slot.");

        var appointment = new Appointment
        {
            FullName = vm.FullName.Trim(),
            Email = vm.Email.Trim(),
            Phone = vm.Phone?.Trim(),
            PracticeAreaId = vm.PracticeAreaId,
            LawyerId = vm.LawyerId,
            PreferredDate = vm.Date.Value.Date.Add(time.ToTimeSpan()),
            Message = vm.Message.Trim()
        };

        _db.Appointments.Add(appointment);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Two people can pass the check above at the same moment.
            // The unique index in AppDbContext is the final safety net.
            return new AppointmentResult(false, "Someone just booked that slot. Please pick another time.");
        }

        return new AppointmentResult(true);
    }
}
