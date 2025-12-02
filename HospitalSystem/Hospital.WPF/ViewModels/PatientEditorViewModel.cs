using Hospital.Business.Models;
using Hospital.Business.Models.People;
using Hospital.Data.Repositories;
using Hospital.WPF.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Hospital.WPF.ViewModels
{
    /// <summary>
    /// ViewModel для окна создания и редактирования пациента.
    /// </summary>
    public class PatientEditorViewModel : BaseViewModel
    {
        public Patient Patient { get; private set; }

        public ObservableCollection<Doctor> AllDoctors { get; } = new();
        private Doctor? _selectedDoctor;
        public Doctor? SelectedDoctor { get => _selectedDoctor; set { _selectedDoctor = value; OnPropertyChanged(); } }

        public ObservableCollection<Department> AllDepartments { get; } = new();
        private Department? _selectedDepartment;
        public Department? SelectedDepartment { get => _selectedDepartment; set { _selectedDepartment = value; OnPropertyChanged(); } }

        public event Action<bool>? CloseRequested;
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private PatientEditorViewModel(Patient patient)
        {
            // Создание копии объекта для редактирования, чтобы избежать
            // изменения оригинального объекта до нажатия "Сохранить".
            Patient = new Patient
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                MiddleName = patient.MiddleName,
                DateOfBirth = patient.Id == Guid.Empty ? new DateTime(1985, 1, 1) : patient.DateOfBirth,
                Diagnosis = patient.Diagnosis,
                DoctorId = patient.DoctorId,
                DepartmentId = patient.DepartmentId,
                Address = patient.Address,
                PhoneNumber = patient.PhoneNumber
            };

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);
        }

        private bool CanSave(object? obj)
        {
            return !string.IsNullOrWhiteSpace(Patient.FirstName) &&
                   !string.IsNullOrWhiteSpace(Patient.LastName);
        }

        private void Save(object? obj)
        {
            // Перед закрытием окна обновляем ID в редактируемом объекте.
            Patient.DoctorId = SelectedDoctor?.Id;
            Patient.DepartmentId = SelectedDepartment?.Id;
            CloseRequested?.Invoke(true);
        }

        private void Cancel(object? obj) => CloseRequested?.Invoke(false);

        public static async Task<PatientEditorViewModel> CreateAsync(Patient patient, IRepository<Doctor> doctorRepository, IRepository<Department> departmentRepository)
        {
            var viewModel = new PatientEditorViewModel(patient);
            await viewModel.LoadDoctorsAsync(doctorRepository);
            await viewModel.LoadDepartmentsAsync(departmentRepository);
            viewModel.SetInitialSelections();
            return viewModel;
        }

        private async Task LoadDoctorsAsync(IRepository<Doctor> doctorRepository)
        {
            var doctors = await doctorRepository.GetAllAsync();
            AllDoctors.Clear();
            foreach (var doc in doctors.OrderBy(doctor => doctor.LastName)) { AllDoctors.Add(doc); }
        }

        private async Task LoadDepartmentsAsync(IRepository<Department> departmentRepository)
        {
            var departments = await departmentRepository.GetAllAsync();
            AllDepartments.Clear();
            foreach (var dep in departments.OrderBy(doctor => doctor.Name)) { AllDepartments.Add(dep); }
        }

        private void SetInitialSelections()
        {
            if (Patient.DoctorId != null)
            {
                SelectedDoctor = AllDoctors.FirstOrDefault(doctor => doctor.Id == Patient.DoctorId);
            }
            if (Patient.DepartmentId != null)
            {
                SelectedDepartment = AllDepartments.FirstOrDefault(doctor => doctor.Id == Patient.DepartmentId);
            }
        }
    }
}