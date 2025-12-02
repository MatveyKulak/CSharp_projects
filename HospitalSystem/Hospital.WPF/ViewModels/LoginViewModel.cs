using Hospital.Business.Models.People;
using Hospital.Services.Interfaces;
using System.Windows;

namespace Hospital.WPF.ViewModels
{
    /// <summary>
    /// ViewModel для окна входа в систему.
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authenticationService;

        private string _username = string.Empty;
        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }

        public LoginViewModel(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Выполняет аутентификацию и возвращает пользователя в случае успеха.
        /// Отображает MessageBox в случае ошибки.
        /// </summary>
        /// <returns>Объект User при успешной аутентификации, иначе null.</returns>
        public async Task<User?> AuthenticateAndGetUserAsync(string password)
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Логин и пароль не могут быть пустыми.", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            var user = await _authenticationService.AuthenticateAsync(Username, password);
            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return user;
        }
    }
}