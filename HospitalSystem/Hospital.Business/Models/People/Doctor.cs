namespace Hospital.Business.Models.People
{
    /// <summary>
    /// Представляет врача, пользователя системы с расширенными правами.
    /// </summary>
    public class Doctor : User
    {
        public string Specialization { get; set; } = string.Empty;

        public Guid? DepartmentId { get; set; }
        public virtual Department? Department { get; set; }

        public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
        public override string DepartmentName => Department?.Name ?? "Не указано";
    }
}