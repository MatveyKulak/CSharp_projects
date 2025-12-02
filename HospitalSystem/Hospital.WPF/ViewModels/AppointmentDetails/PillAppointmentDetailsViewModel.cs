namespace Hospital.WPF.ViewModels.AppointmentDetails
{
    /// <summary>
    /// ViewModel для отображения и редактирования уникальных полей
    /// назначения таблеток.
    /// </summary>
    public class PillAppointmentDetailsViewModel : BaseViewModel
    {
        private string _medicationName = string.Empty;
        public string MedicationName { get => _medicationName; set { _medicationName = value; OnPropertyChanged(); } }

        private string _dosage = string.Empty;
        public string Dosage { get => _dosage; set { _dosage = value; OnPropertyChanged(); } }

        private string _frequency = string.Empty;
        public string Frequency { get => _frequency; set { _frequency = value; OnPropertyChanged(); } }

        private int _days;
        public int Days { get => _days; set { _days = value; OnPropertyChanged(); } }
    }
}