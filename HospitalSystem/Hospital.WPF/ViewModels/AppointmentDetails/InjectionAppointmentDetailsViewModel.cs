namespace Hospital.WPF.ViewModels.AppointmentDetails
{
    /// <summary>
    /// ViewModel для отображения и редактирования уникальных полей
    /// назначения-инъекции.
    /// </summary>
    public class InjectionAppointmentDetailsViewModel : BaseViewModel
    {
        private string _medicationName = string.Empty;
        /// <summary>
        /// Название препарата.
        /// </summary>
        public string MedicationName
        {
            get => _medicationName;
            set { _medicationName = value; OnPropertyChanged(); }
        }

        private string _dosage = string.Empty;
        /// <summary>
        /// Дозировка препарата.
        /// </summary>
        public string Dosage
        {
            get => _dosage;
            set { _dosage = value; OnPropertyChanged(); }
        }

        private int _quantity;
        /// <summary>
        /// Количество инъекций.
        /// </summary>
        public int Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(); }
        }
    }
}