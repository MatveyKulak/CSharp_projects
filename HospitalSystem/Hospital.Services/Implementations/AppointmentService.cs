using Hospital.Business.Models.Medical;
using Hospital.Business.Models.People;
using Hospital.Data.Repositories;
using Hospital.Services.Interfaces;

namespace Hospital.Services.Implementations
{
    /// <summary>
    /// Реализует сервис для управления медицинскими назначениями.
    /// </summary>
    public class AppointmentService : IAppointmentService
    {
        private readonly IRepository<Appointment> _appointmentRepository;

        public AppointmentService(IRepository<Appointment> appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<MedicationAppointment> AddMedicationAppointmentAsync(Patient patient, MedicationAppointment appointment, Guid doctorId)
        {
            await AddAppointmentInternal(patient, appointment, doctorId);
            return appointment;
        }

        public async Task<DiagnosticAppointment> AddDiagnosticAppointmentAsync(Patient patient, DiagnosticAppointment appointment, Guid doctorId)
        {
            await AddAppointmentInternal(patient, appointment, doctorId);
            return appointment;
        }

        public async Task<ProphylacticAppointment> AddProphylacticAppointmentAsync(Patient patient, ProphylacticAppointment appointment, Guid doctorId)
        {
            await AddAppointmentInternal(patient, appointment, doctorId);
            return appointment;
        }

        /// <summary>
        /// Внутренний обобщенный метод для добавления любого типа назначения.
        /// </summary>
        private async Task AddAppointmentInternal<T>(Patient patient, T appointment, Guid doctorId) where T : Appointment
        {
            if (patient.MedicalRecord == null)
            {
                throw new InvalidOperationException("У пациента отсутствует медицинская карта.");
            }

            appointment.Id = Guid.NewGuid();
            appointment.AppointmentDate = DateTime.Now;
            appointment.PrescribingDoctorId = doctorId;
            appointment.MedicalRecordId = patient.MedicalRecord.Id;

            await _appointmentRepository.AddAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();
        }

        public async Task UpdateAppointmentAsync(Appointment appointment)
        {
            _appointmentRepository.Update(appointment);
            await _appointmentRepository.SaveChangesAsync();
        }

        public async Task DeleteAppointmentAsync(Guid appointmentId)
        {
            var appointmentToDelete = await _appointmentRepository.GetByIdAsync(appointmentId);
            if (appointmentToDelete != null)
            {
                _appointmentRepository.Remove(appointmentToDelete);
                await _appointmentRepository.SaveChangesAsync();
            }
        }
    }
}