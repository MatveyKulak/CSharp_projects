using Hospital.Business.Models.People;
using System.Collections.ObjectModel;

namespace Hospital.Business.Models.Medical
{
    /// <summary>
    /// Электронная медицинская карта (ЭМК) пациента.
    /// Содержит историю госпитализации и все медицинские назначения.
    /// </summary>
    public class MedicalRecord
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public virtual Patient Patient { get; set; } = null!;
        public DateTime HospitalizationDate { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }

        public MedicalRecord()
        {
            Appointments = new ObservableCollection<Appointment>();
        }
    }
}