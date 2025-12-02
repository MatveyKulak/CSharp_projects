namespace Hospital.Business.Models.Medical
{
    /// <summary>
    /// Представляет диагностическое назначение (например, УЗИ, МРТ).
    /// </summary>
    public class DiagnosticAppointment : Appointment
    {
        public string ProcedureName { get; set; } = string.Empty;
        public string? Results { get; set; }

        public override string Summary =>
            $"{ProcedureName}{(string.IsNullOrWhiteSpace(Notes) ? "" : $" ({Notes})")}";
    }
}