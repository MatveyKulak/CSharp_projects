using Hospital.Business.Models.People;

namespace Hospital.Services.Interfaces
{
    /// <summary>
    /// Определяет контракт для сервиса управления данными пациентов.
    /// </summary>
    public interface IPatientService
    {
        /// <summary>
        /// Получает всех пациентов со связанными данными.
        /// </summary>
        Task<IEnumerable<Patient>> GetAllPatientsAsync();
        Task AddPatientAsync(Patient patient);
        Task UpdatePatientAsync(Patient patient);
        Task DeletePatientAsync(Guid patientId);
    }
}