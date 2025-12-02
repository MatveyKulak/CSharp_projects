using Hospital.WPF.ViewModels;
using System.Windows;

namespace Hospital.WPF.Views
{
    public partial class UserEditorView : Window
    {
        /// <summary>
        /// Окно для создания и редактирования данных пользователя.
        /// </summary>
        public UserEditorView(UserEditorViewModel viewModel)
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