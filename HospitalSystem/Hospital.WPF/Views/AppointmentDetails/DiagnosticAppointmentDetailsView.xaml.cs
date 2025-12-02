using System.Windows.Controls;

namespace Hospital.WPF.Views.AppointmentDetails
{
    /// <summary>
    /// Представление (UserControl) для отображения полей диагностического назначения.
    /// Логика полностью управляется через DataContext (DiagnosticAppointmentDetailsViewModel).
    /// </summary>
    public partial class DiagnosticAppointmentDetailsView : UserControl
    {
        public DiagnosticAppointmentDetailsView()
        {
            InitializeComponent();
        }
    }
}