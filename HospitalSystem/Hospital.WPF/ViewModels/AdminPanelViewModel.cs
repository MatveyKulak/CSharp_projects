using Hospital.Business.Models;
using Hospital.Business.Models.People;
using Hospital.Data.Repositories;
using Hospital.Services.Interfaces;
using Hospital.WPF.Commands;
using Hospital.WPF.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Hospital.WPF.ViewModels
{
    /// <summary>
    /// ViewModel для панели администратора, управляющая пользователями и отделениями.
    /// </summary>
    public class AdminPanelViewModel : BaseViewModel
    {
        // --- Зависимости ---
        private readonly IUserService _userService;
        private readonly IDepartmentService _departmentService;
        private readonly IDialogService _dialogService;
        private readonly IServiceProvider _serviceProvider; // Для "ленивого" получения зависимостей

        // --- Состояние для Пользователей ---
        private readonly ObservableCollection<User> _allUsers = new();
        public ObservableCollection<User> FilteredUsers { get; } = new();

        private string _searchText = string.Empty;
        public string SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(); FilterUsers(); } }

        private User? _selectedUser;
        public User? SelectedUser { get => _selectedUser; set { _selectedUser = value; OnPropertyChanged(); DeleteUserCommand.RaiseCanExecuteChanged(); EditUserCommand.RaiseCanExecuteChanged(); } }

        // --- Состояние для Отделений ---
        public ObservableCollection<Department> Departments { get; } = new();
        private Department? _selectedDepartment;
        public Department? SelectedDepartment { get => _selectedDepartment; set { _selectedDepartment = value; OnPropertyChanged(); EditDepartmentCommand.RaiseCanExecuteChanged(); DeleteDepartmentCommand.RaiseCanExecuteChanged(); } }

        // --- Команды ---
        public RelayCommand AddUserCommand { get; }
        public RelayCommand EditUserCommand { get; }
        public RelayCommand DeleteUserCommand { get; }
        public RelayCommand AddDepartmentCommand { get; }
        public RelayCommand EditDepartmentCommand { get; }
        public RelayCommand DeleteDepartmentCommand { get; }

        public AdminPanelViewModel(IUserService userService, IDepartmentService departmentService, IDialogService dialogService, IServiceProvider serviceProvider)
        {
            _userService = userService;
            _departmentService = departmentService;
            _dialogService = dialogService;
            _serviceProvider = serviceProvider;

            AddUserCommand = new RelayCommand(AddUser);
            EditUserCommand = new RelayCommand(EditUser, CanEditOrDeleteUser);
            DeleteUserCommand = new RelayCommand(DeleteUser, CanEditOrDeleteUser);
            AddDepartmentCommand = new RelayCommand(AddDepartment);
            EditDepartmentCommand = new RelayCommand(EditDepartment, CanEditOrDeleteDepartment);
            DeleteDepartmentCommand = new RelayCommand(DeleteDepartment, CanEditOrDeleteDepartment);
        }

        /// <summary>
        /// Асинхронно загружает все необходимые данные для панели.
        /// </summary>
        public async Task LoadDataAsync()
        {
            await LoadUsersAsync();
            await LoadDepartmentsAsync();
        }

        private async Task LoadUsersAsync()
        {
            var users = await _userService.GetAllUsersAsync();
            _allUsers.Clear();
            foreach (var user in users) { _allUsers.Add(user); }
            FilterUsers();
        }

        private void FilterUsers()
        {
            FilteredUsers.Clear();
            var searchTextLower = SearchText.ToLower();
            var filtered = string.IsNullOrWhiteSpace(searchTextLower) ? _allUsers : _allUsers.Where(u => u.FullName.ToLower().Contains(searchTextLower) || u.Username.ToLower().Contains(searchTextLower));
            foreach (var user in filtered) { FilteredUsers.Add(user); }
        }

        private bool CanEditOrDeleteUser(object? obj) => SelectedUser != null;
        private void AddUser(object? obj) => OpenUserEditor(new Doctor());
        private void EditUser(object? obj) { if (SelectedUser != null) OpenUserEditor(SelectedUser); }

        private async void OpenUserEditor(User user)
        {
            var departmentRepository = _serviceProvider.GetRequiredService<IRepository<Department>>();
            var userEditorViewModel = await UserEditorViewModel.CreateAsync(user, departmentRepository);
            var result = _dialogService.ShowDialog(userEditorViewModel);

            if (result == true)
            {
                var editedUser = userEditorViewModel.User;
                var password = userEditorViewModel.Password;
                var role = userEditorViewModel.SelectedRole;

                if (userEditorViewModel.SelectedDepartment != null)
                {
                    editedUser.TempDepartmentId = userEditorViewModel.SelectedDepartment.Id;
                }

                try
                {
                    if (userEditorViewModel.IsNewUser)
                    {
                        await _userService.AddUserAsync(editedUser, password, role);
                    }
                    else
                    {
                        // При редактировании тоже нужно обновить DepartmentId
                        if (editedUser is Doctor doc) doc.DepartmentId = editedUser.TempDepartmentId;
                        else if (editedUser is Nurse nur) nur.DepartmentId = editedUser.TempDepartmentId;
                        await _userService.UpdateUserAsync(editedUser, password, role);
                    }
                    await LoadUsersAsync();
                }
                catch (Exception ex) { MessageBox.Show($"Не удалось сохранить пользователя. Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
        }

        private async void DeleteUser(object? obj)
        {
            if (SelectedUser == null) return;
            var result = MessageBox.Show($"Вы уверены, что хотите удалить пользователя {SelectedUser.FullName}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;
            try
            {
                await _userService.DeleteUserAsync(SelectedUser.Id);
                _allUsers.Remove(SelectedUser);
                FilterUsers();
                MessageBox.Show("Пользователь успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) { MessageBox.Show($"Не удалось удалить пользователя. Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private async Task LoadDepartmentsAsync()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            Departments.Clear();
            foreach (var department in departments) { Departments.Add(department); }
        }

        private bool CanEditOrDeleteDepartment(object? obj) => SelectedDepartment != null;

        private async void AddDepartment(object? obj)
        {
            string name = Interaction.InputBox("Введите название нового отделения:", "Добавить отделение", "");
            if (!string.IsNullOrWhiteSpace(name))
            {
                var newDepartment = new Department { Name = name };
                await _departmentService.AddDepartmentAsync(newDepartment);
                await LoadDepartmentsAsync();
            }
        }

        private async void EditDepartment(object? obj)
        {
            if (SelectedDepartment == null) return;
            string name = Interaction.InputBox("Введите новое название отделения:", "Редактировать отделение", SelectedDepartment.Name);
            if (!string.IsNullOrWhiteSpace(name) && name != SelectedDepartment.Name)
            {
                SelectedDepartment.Name = name;
                await _departmentService.UpdateDepartmentAsync(SelectedDepartment);
                await LoadDepartmentsAsync();
            }
        }

        private async void DeleteDepartment(object? obj)
        {
            if (SelectedDepartment == null) return;
            var result = MessageBox.Show($"Вы уверены, что хотите удалить отделение '{SelectedDepartment.Name}'? Врачи и пациенты этого отделения потеряют привязку.", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;
            await _departmentService.DeleteDepartmentAsync(SelectedDepartment.Id);
            await LoadDepartmentsAsync();
        }
    }
}