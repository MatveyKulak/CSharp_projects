using System.Windows.Controls;

namespace Hospital.WPF.Views
{
    /// <summary>
    /// Представление (UserControl) для отображения списка пациентов.
    /// Логика полностью управляется через DataContext (PatientListViewModel).
    /// </summary>
    public partial class PatientListView : UserControl
    {
        public PatientListView()
        {
            InitializeComponent();
        }
    }
}