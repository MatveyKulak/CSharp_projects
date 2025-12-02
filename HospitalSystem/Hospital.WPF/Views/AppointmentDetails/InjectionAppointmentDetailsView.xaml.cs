using System.Windows.Controls;

namespace Hospital.WPF.Views.AppointmentDetails
{
    /// <summary>
    /// Представление (UserControl) для отображения полей инъекционного назначения.
    /// Логика полностью управляется через DataContext (InjectionAppointmentDetailsViewModel).
    /// </summary>
    public partial class InjectionAppointmentDetailsView : UserControl
    {
        public InjectionAppointmentDetailsView()
        {
            InitializeComponent();
        }
    }
}