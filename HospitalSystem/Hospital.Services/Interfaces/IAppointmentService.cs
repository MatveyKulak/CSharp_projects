using Hospital.Business.Models.Medical;
using Hospital.Business.Models.People;

namespace Hospital.Services.Interfaces
{
    /// <summary>
    /// Определяет контракт для сервиса управления медицинскими назначениями.
    /// </summary>
    public interface IAppointmentService
    {
        Task<MedicationAppointment> AddMedicationAppointmentAsync(Patient patient, MedicationAppointment appointment, Guid doctorId);
        Task<DiagnosticAppointment> AddDiagnosticAppointmentAsync(Patient patient, DiagnosticAppointment appointment, Guid doctorId);
        Task<ProphylacticAppointment> AddProphylacticAppointmentAsync(Patient patient, ProphylacticAppointment appointment, Guid doctorId);
        Task UpdateAppointmentAsync(Appointment appointment);
        Task DeleteAppointmentAsync(Guid appointmentId);
    }
}