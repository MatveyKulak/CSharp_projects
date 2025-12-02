namespace Hospital.WPF.ViewModels.AppointmentDetails
{
    /// <summary>
    /// ViewModel для отображения и редактирования уникальных полей
    /// профилактического назначения.
    /// </summary>
    public class ProphylacticAppointmentDetailsViewModel : BaseViewModel
    {
        private string _procedureName = string.Empty;
        public string ProcedureName
        {
            get => _procedureName;
            set { _procedureName = value; OnPropertyChanged(); }
        }
    }
}