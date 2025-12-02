namespace Hospital.WPF.ViewModels.AppointmentDetails
{
    /// <summary>
    /// ViewModel для отображения и редактирования уникальных полей
    /// диагностического назначения.
    /// </summary>
    public class DiagnosticAppointmentDetailsViewModel : BaseViewModel
    {
        private string _procedureName = string.Empty;
        public string ProcedureName
        {
            get => _procedureName;
            set { _procedureName = value; OnPropertyChanged(); }
        }
    }
}