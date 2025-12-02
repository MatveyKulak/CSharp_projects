namespace Hospital.Business.Models.Medical
{
    /// <summary>
    /// Представляет профилактическое назначение (например, лечебная физкультура).
    /// </summary>
    public class ProphylacticAppointment : Appointment
    {
        public string ProcedureName { get; set; } = string.Empty;

        public override string Summary =>
            $"{ProcedureName}{(string.IsNullOrWhiteSpace(Notes) ? "" : $" ({Notes})")}";
    }
}