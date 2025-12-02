using Hospital.WPF.ViewModels;
using System.Windows;

namespace Hospital.WPF.Views
{
    /// <summary>
    /// Окно для отображения детальной информации о пациенте.
    /// </summary>
    public partial class PatientDetailsView : Window
    {
        public PatientDetailsView(PatientDetailsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}