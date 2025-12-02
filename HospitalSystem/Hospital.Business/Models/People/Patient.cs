using Hospital.Business.Models.Medical;

namespace Hospital.Business.Models.People
{
    /// <summary>
    /// Представляет пациента, находящегося на лечении в больнице.
    /// Не является пользователем системы (не может входить в систему).
    /// </summary>
    public class Patient : Person
    {
        public string Diagnosis { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public Guid? DepartmentId { get; set; }
        public virtual Department? Department { get; set; }

        public Guid? DoctorId { get; set; }
        public virtual Doctor? AssignedDoctor { get; set; }

        public virtual MedicalRecord MedicalRecord { get; set; } = null!;
    }
}