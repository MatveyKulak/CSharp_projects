using Hospital.WPF.ViewModels;
using System.Windows;

namespace Hospital.WPF.Views
{
    /// <summary>
    /// Окно для добавления нового медицинского назначения.
    /// </summary>
    public partial class AddAppointmentView : Window
    {
        public AddAppointmentView(AddAppointmentViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            // Подписываемся на событие из ViewModel, чтобы View могло отреагировать на запрос о закрытии.
            viewModel.CloseRequested += (success) =>
            {
                // Устанавливаем результат диалога, который будет возвращен методом ShowDialog().
                DialogResult = success;
                Close();
            };
        }
    }
}