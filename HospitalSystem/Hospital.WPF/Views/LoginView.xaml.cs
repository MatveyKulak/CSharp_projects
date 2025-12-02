using Hospital.Business.Models.People;
using Hospital.WPF.ViewModels;
using System.Windows;

namespace Hospital.WPF.Views
{
    /// <summary>
    /// Окно для входа в систему.
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginViewModel ViewModel { get; }

        /// <summary>
        /// Свойство для хранения успешно аутентифицированного пользователя.
        /// Используется в App.xaml.cs для получения результата работы окна.
        /// </summary>
        public User? AuthenticatedUser { get; private set; }

        public LoginView(LoginViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = ViewModel;
        }

        // Обработчик события нажатия на кнопку "Войти".
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем пароль напрямую из элемента управления PasswordBox.
            // Делегируем всю логику аутентификации в ViewModel.
            var user = await ViewModel.AuthenticateAndGetUserAsync(PasswordInput.Password);

            // Если ViewModel вернула пользователя, значит аутентификация прошла успешно.
            if (user != null)
            {
                AuthenticatedUser = user;
                // Закрываем окно, чтобы передать управление обратно в App.xaml.cs.
                this.Close();
            }
        }
    }
}