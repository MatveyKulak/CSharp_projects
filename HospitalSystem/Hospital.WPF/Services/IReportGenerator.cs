using Hospital.Business.Models.People;

namespace Hospital.WPF.Services
{
    /// <summary>
    /// Определяет контракт для сервиса генерации отчетов.
    /// </summary>
    public interface IReportGenerator
    {
        /// <summary>
        /// Генерирует и открывает PDF-отчет "Выписной эпикриз" для пациента.
        /// </summary>
        void GenerateDischargeSummary(Patient patient);
    }
}