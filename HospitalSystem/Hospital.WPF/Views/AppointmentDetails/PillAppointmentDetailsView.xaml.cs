using System.Windows.Controls;

namespace Hospital.WPF.Views.AppointmentDetails
{
    /// <summary>
    /// Представление (UserControl) для отображения полей таблеточного назначения.
    /// Логика полностью управляется через DataContext (PillAppointmentDetailsViewModel).
    /// </summary>
    public partial class PillAppointmentDetailsView : UserControl
    {
        public PillAppointmentDetailsView()
        {
            InitializeComponent();
        }
    }
}