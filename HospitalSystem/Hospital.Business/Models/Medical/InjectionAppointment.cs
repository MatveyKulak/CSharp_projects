namespace Hospital.Business.Models.Medical
{
    /// <summary>
    /// Назначение инъекции.
    /// </summary>
    public class InjectionAppointment : MedicationAppointment
    {
        public int Quantity { get; set; }

        public override string Summary =>
            $"{MedicationName}, {Dosage} - {Quantity} инъекций {(string.IsNullOrWhiteSpace(Notes) ? "" : $" ({Notes})")}";
    }
}