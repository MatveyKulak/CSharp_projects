using System.Windows.Controls;

namespace Hospital.WPF.Views.AppointmentDetails
{
    /// <summary>
    /// Представление (UserControl) для отображения полей профилактического назначения.
    /// Логика полностью управляется через DataContext (ProphylacticAppointmentDetailsViewModel).
    /// </summary>
    public partial class ProphylacticAppointmentDetailsView : UserControl
    {
        public ProphylacticAppointmentDetailsView()
        {
            InitializeComponent();
        }
    }
}