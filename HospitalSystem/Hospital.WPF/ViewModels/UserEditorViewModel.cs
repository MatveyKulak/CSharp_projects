using Hospital.Business.Models;
using Hospital.Business.Models.People;
using Hospital.Data.Repositories;
using Hospital.WPF.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Hospital.WPF.ViewModels
{
    /// <summary>
    /// ViewModel для окна создания и редактирования пользователя.
    /// </summary>
    public class UserEditorViewModel : BaseViewModel
    {
        public User User { get; set; }
        public string Password { get; set; } = string.Empty;
        public List<string> Roles { get; } = new List<string> { "Врач", "Медсестра", "Администратор" };

        private string _selectedRole;
        public string SelectedRole
        {
            get => _selectedRole;
            set
            {
                _selectedRole = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsStaffRoleSelected)); // Уведомляем об изменении видимости
            }
        }

        public bool IsNewUser { get; }
        public event Action<bool>? CloseRequested;
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ObservableCollection<Department> AllDepartments { get; } = new();
        private Department? _selectedDepartment;
        public Department? SelectedDepartment
        {
            get => _selectedDepartment;
            set { _selectedDepartment = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// Управляет видимостью списка отделений в зависимости от выбранной роли.
        /// </summary>
        public bool IsStaffRoleSelected => SelectedRole == "Врач" || SelectedRole == "Медсестра";

        public UserEditorViewModel(User user)
        {
            User = user; // Мы работаем с оригинальным объектом, чтобы сохранить DepartmentId
            _selectedRole = user.Role;
            IsNewUser = user.Id == Guid.Empty;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);
        }

        private bool CanSave(object? obj)
        {
            if (string.IsNullOrWhiteSpace(User.FirstName) || string.IsNullOrWhiteSpace(User.LastName) || string.IsNullOrWhiteSpace(User.Username))
                return false;
            if (IsNewUser && string.IsNullOrWhiteSpace(Password))
                return false;
            return true;
        }

        private void Save(object? obj)
        {
            // При сохранении нужно будет обновить DepartmentId в самом объекте User,
            // но так как User абстрактный, это нужно делать в UserService после определения типа.
            // Здесь мы просто закрываем окно.
            CloseRequested?.Invoke(true);
        }

        private void Cancel(object? obj) => CloseRequested?.Invoke(false);

        public static async Task<UserEditorViewModel> CreateAsync(User user, IRepository<Department> departmentRepository)
        {
            var viewModel = new UserEditorViewModel(user);
            await viewModel.LoadDepartmentsAsync(departmentRepository);
            viewModel.SetInitialDepartment();
            return viewModel;
        }

        private async Task LoadDepartmentsAsync(IRepository<Department> departmentRepository)
        {
            var departments = await departmentRepository.GetAllAsync();
            AllDepartments.Clear();
            foreach (var dep in departments.OrderBy(doctor => doctor.Name))
            {
                AllDepartments.Add(dep);
            }
        }

        private void SetInitialDepartment()
        {
            Guid? departmentId = null;
            if (User is Doctor doc) departmentId = doc.DepartmentId;
            else if (User is Nurse nur) departmentId = nur.DepartmentId;

            if (departmentId != null)
            {
                SelectedDepartment = AllDepartments.FirstOrDefault(doctor => doctor.Id == departmentId);
            }
        }
    }
}