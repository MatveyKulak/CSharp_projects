namespace Hospital.Business.Models.Medical
{
    /// <summary>
    /// Базовый абстрактный класс для медикаментозных назначений (таблетки, инъекции).
    /// </summary>
    public abstract class MedicationAppointment : Appointment
    {
        public string MedicationName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
    }
}