using Hospital.Business.Models.People;

namespace Hospital.Business.Models
{
    /// <summary>
    /// Представляет отделение больницы (например, Хирургия, Терапия).
    /// </summary>
    public class Department
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
        public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
        public virtual ICollection<Nurse> Nurses { get; set; } = new List<Nurse>();
    }
}