using Hospital.WPF.ViewModels;
using System.Windows;

namespace Hospital.WPF.Views
{
    /// <summary>
    /// Окно для создания и редактирования данных пациента.
    /// </summary>
    public partial class PatientEditorView : Window
    {
        public PatientEditorView(PatientEditorViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            // Подписываемся на событие из ViewModel для закрытия окна.
            viewModel.CloseRequested += (success) =>
            {
                DialogResult = success;
                Close();
            };
        }
    }
}