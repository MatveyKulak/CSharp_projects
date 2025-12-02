using Hospital.Business.Enums;
using Hospital.Business.Models.People;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Hospital.Business.Models.Medical
{
    /// <summary>
    /// Базовый абстрактный класс для всех медицинских назначений.
    /// Определяет общие свойства и поведение.
    /// </summary>
    public abstract class Appointment : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected Appointment()
        {
            Status = AppointmentStatus.Scheduled;
        }

        public Guid Id { get; set; }
        public Guid MedicalRecordId { get; set; }
        public virtual MedicalRecord MedicalRecord { get; set; } = null!;
        public DateTime AppointmentDate { get; set; }
        public string Notes { get; set; } = string.Empty;

        private AppointmentStatus _status;
        public AppointmentStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StatusText)); // Уведомляем UI, что и StatusText изменился
                }
            }
        }

        public Guid PrescribingDoctorId { get; set; }
        public virtual Doctor PrescribingDoctor { get; set; } = null!;

        public string AppointmentType
        {
            get
            {
                return this switch
                {
                    MedicationAppointment => "Медикаментозное",
                    DiagnosticAppointment => "Диагностическое",
                    ProphylacticAppointment => "Профилактическое",
                    _ => GetType().Name
                };
            }
        }

        /// <summary>
        /// Возвращает русское название для текущего статуса.
        /// </summary>
        public string StatusText
        {
            get
            {
                return Status switch
                {
                    AppointmentStatus.Scheduled => "Запланировано",
                    AppointmentStatus.Completed => "Выполнено",
                    AppointmentStatus.Canceled => "Отменено",
                    _ => Status.ToString()
                };
            }
        }

        /// <summary>
        /// Предоставляет краткое описание назначения для отображения в списках.
        /// </summary>
        public abstract string Summary { get; }
    }
}