using Hospital.Business.Models.People;
using Hospital.Services.Interfaces;
using Hospital.WPF.Commands;
using Hospital.WPF.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Input;

namespace Hospital.WPF.ViewModels
{
    /// <summary>
    /// Основная ViewModel приложения, управляющая отображением контента после входа.
    /// Выступает в роли "оболочки" (Shell) для других ViewModel.
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly User _currentUser;

        /// <summary>
        /// Событие, сигнализирующее о необходимости выхода из системы (возврата к экрану входа).
        /// </summary>
        public event Action? LogoutRequested;

        // "Дочерние" ViewModel для разных панелей
        public PatientListViewModel? PatientListVM { get; private set; }
        public AdminPanelViewModel? AdminPanelVM { get; private set; }

        // Свойства для управления видимостью панелей на основе роли
        public bool IsAdminViewVisible => _currentUser is Administrator;
        public bool IsPatientViewVisible => !IsAdminViewVisible;
        public string WelcomeMessage => $"Добро пожаловать, {_currentUser.FirstName} {_currentUser.LastName}!";

        public ICommand LogoutCommand { get; }

        private MainViewModel(IServiceProvider serviceProvider, User currentUser)
        {
            _serviceProvider = serviceProvider;
            _currentUser = currentUser;
            LogoutCommand = new RelayCommand(Logout);
        }

        private void Logout(object? obj) => LogoutRequested?.Invoke();

        /// <summary>
        /// Асинхронный статический фабричный метод для создания и инициализации ViewModel.
        /// </summary>
        public static async Task<MainViewModel> CreateAsync(IServiceProvider serviceProvider, User currentUser)
        {
            var viewModel = new MainViewModel(serviceProvider, currentUser);
            await viewModel.LoadDataForRoleAsync();
            return viewModel;
        }

        /// <summary>
        /// Загружает данные и инициализирует дочерние ViewModel в зависимости от роли текущего пользователя.
        /// </summary>
        private async Task LoadDataForRoleAsync()
        {
            if (_currentUser is Administrator)
            {
                // Получаем зависимости "на лету" с помощью Service Locator (IServiceProvider)
                var userService = _serviceProvider.GetRequiredService<IUserService>();
                var departmentService = _serviceProvider.GetRequiredService<IDepartmentService>();
                var dialogService = _serviceProvider.GetRequiredService<IDialogService>();

                AdminPanelVM = new AdminPanelViewModel(userService, departmentService, dialogService, _serviceProvider);
                await AdminPanelVM.LoadDataAsync();
            }
            else
            {
                var dialogService = _serviceProvider.GetRequiredService<IDialogService>();
                PatientListVM = new PatientListViewModel(_serviceProvider, _currentUser, dialogService);
                await PatientListVM.LoadDataAsync();
            }
        }
    }
}