namespace Hospital.Business.Models.Medical
{
    /// <summary>
    /// Назначение таблеток.
    /// </summary>
    public class PillAppointment : MedicationAppointment
    {
        public int Days { get; set; }

        public override string Summary =>
            $"{MedicationName}, {Dosage}, {Frequency} - {Days} дней {(string.IsNullOrWhiteSpace(Notes) ? "" : $" ({Notes})")}";
    }
}