using LawFirm.Web.ViewModels;

namespace LawFirm.Web.Services;

public record AppointmentResult(bool Success, string? Error = null);

public interface IAppointmentService
{
    Task<IReadOnlyList<string>> GetAvailableSlotsAsync(int? lawyerId, DateTime date);
    Task<AppointmentResult> CreateAsync(AppointmentCreateVm vm);
}
