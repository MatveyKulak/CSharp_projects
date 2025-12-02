using System.Windows.Controls;

namespace Hospital.WPF.Views
{
    /// <summary>
    /// Представление (UserControl) для панели администратора.
    /// Логика полностью управляется через DataContext (AdminPanelViewModel).
    /// </summary>
    public partial class AdminPanelView : UserControl
    {
        public AdminPanelView()
        {
            InitializeComponent();
        }
    }
}