using Hospital.WPF.ViewModels;
using Hospital.WPF.Views;
using System.Windows;
using System.Windows.Controls;

namespace Hospital.WPF
{
    /// <summary>
    /// Главное окно приложения, выступающее в роли оболочки (Shell).
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            this.Loaded += MainWindow_Loaded;
        }

        /// <summary>
        /// Динамически загружает нужный UserControl (AdminPanelView или PatientListView)
        /// в зависимости от роли пользователя, определенной в MainViewModel.
        /// </summary>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                UserControl content = null;

                if (vm.IsAdminViewVisible)
                {
                    content = new AdminPanelView { DataContext = vm.AdminPanelVM };
                }
                else
                {
                    content = new PatientListView { DataContext = vm.PatientListVM };
                }

                Grid.SetRow(content, 1);
                MainGrid.Children.Add(content);
            }
        }
    }
}