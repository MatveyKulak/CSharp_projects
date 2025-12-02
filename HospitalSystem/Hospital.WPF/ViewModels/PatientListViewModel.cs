using Hospital.Business.Models;
using Hospital.Business.Models.People;
using Hospital.Data.Repositories;
using Hospital.Services.Interfaces;
using Hospital.WPF.Commands;
using Hospital.WPF.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;

namespace Hospital.WPF.ViewModels
{
    /// <summary>
    /// ViewModel для основного экрана врача/медсестры, отображающая список пациентов.
    /// </summary>
    public class PatientListViewModel : BaseViewModel
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IPatientService _patientService;
        private readonly IReportGenerator _reportGenerator;
        private readonly IDialogService _dialogService;
        private readonly User _currentUser;

        private readonly ObservableCollection<Patient> _allPatients = new();
        public ObservableCollection<Patient> FilteredPatients { get; } = new();

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); FilterPatients(); }
        }

        private Patient? _selectedPatient;
        public Patient? SelectedPatient
        {
            get => _selectedPatient;
            set
            {
                _selectedPatient = value;
                OnPropertyChanged();
                EditPatientCommand.RaiseCanExecuteChanged();
                DeletePatientCommand.RaiseCanExecuteChanged();
                OpenPatientDetailsCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Определяет, имеет ли текущий пользователь права на добавление/редактирование/удаление пациентов.
        /// </summary>
        public bool CanManagePatients => _currentUser is Doctor || _currentUser is Administrator;

        /// <summary>
        /// Определяет, имеет ли текущий пользователь право просматривать карточку пациента.
        /// </summary>
        public bool CanViewPatientDetails => _currentUser is Doctor || _currentUser is Administrator || _currentUser is Nurse;
        public RelayCommand AddPatientCommand { get; }
        public RelayCommand EditPatientCommand { get; }
        public RelayCommand DeletePatientCommand { get; }
        public RelayCommand OpenPatientDetailsCommand { get; }

        public PatientListViewModel(IServiceProvider serviceProvider, User currentUser, IDialogService dialogService)
        {
            _serviceProvider = serviceProvider;
            _currentUser = currentUser;
            _patientService = _serviceProvider.GetRequiredService<IPatientService>();
            _reportGenerator = _serviceProvider.GetRequiredService<IReportGenerator>();
            _dialogService = dialogService;

            AddPatientCommand = new RelayCommand(AddPatient, _ => CanManagePatients);
            EditPatientCommand = new RelayCommand(EditPatient, CanEditOrDeletePatient);
            DeletePatientCommand = new RelayCommand(DeletePatient, CanEditOrDeletePatient);
            OpenPatientDetailsCommand = new RelayCommand(OpenPatientDetails, CanOpenPatientDetails);
        }

        public async Task LoadDataAsync()
        {
            var patients = await _patientService.GetAllPatientsAsync();
            _allPatients.Clear();
            foreach (var patient in patients) { _allPatients.Add(patient); }
            FilterPatients();
        }

        private void FilterPatients()
        {
            FilteredPatients.Clear();
            var searchTextLower = SearchText.ToLower();
            var filtered = string.IsNullOrWhiteSpace(searchTextLower)
                ? _allPatients
                : _allPatients.Where(p =>
                    p.FullName.ToLower().Contains(searchTextLower) ||
                    (p.Diagnosis?.ToLower().Contains(searchTextLower) ?? false));
            foreach (var patient in filtered) { FilteredPatients.Add(patient); }
        }

        private bool CanEditOrDeletePatient(object? obj) => CanManagePatients && SelectedPatient != null;
        private bool CanOpenPatientDetails(object? obj) => CanViewPatientDetails && SelectedPatient != null;

        private async void DeletePatient(object? obj)
        {
            if (SelectedPatient == null) return;
            // Бизнес-логика: перед удалением пациента обязательно генерируется отчет.
            var result = MessageBox.Show($"Сформировать выписной эпикриз и выписать пациента {SelectedPatient.FullName}?", "Подтверждение выписки", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;
            try
            {
                _reportGenerator.GenerateDischargeSummary(SelectedPatient);
                await _patientService.DeletePatientAsync(SelectedPatient.Id);
                _allPatients.Remove(SelectedPatient);
                FilteredPatients.Remove(SelectedPatient);

                MessageBox.Show("Пациент успешно выписан.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось сформировать отчет или выписать пациента. Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditPatient(object? obj)
        {
            if (SelectedPatient == null) return;
            OpenPatientEditor(SelectedPatient);
        }

        private void AddPatient(object? obj)
        {
            OpenPatientEditor(new Patient());
        }

        private async void OpenPatientEditor(Patient patient)
        {
            var doctorRepository = _serviceProvider.GetRequiredService<IRepository<Doctor>>();
            var departmentRepository = _serviceProvider.GetRequiredService<IRepository<Department>>();

            var patientToEdit = new Patient
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                MiddleName = patient.MiddleName,
                DateOfBirth = patient.DateOfBirth,
                Diagnosis = patient.Diagnosis,
                DoctorId = patient.DoctorId,
                DepartmentId = patient.DepartmentId
            };

            var patientEditorViewModel = await PatientEditorViewModel.CreateAsync(patientToEdit, doctorRepository, departmentRepository);
            var result = _dialogService.ShowDialog(patientEditorViewModel);

            if (result == true)
            {
                var editedPatient = patientEditorViewModel.Patient;
                bool isNew = editedPatient.Id == Guid.Empty;

                if (isNew)
                {
                    await _patientService.AddPatientAsync(editedPatient);
                }
                else
                {
                    await _patientService.UpdatePatientAsync(editedPatient);
                }
                await LoadDataAsync();

                var successMessage = isNew ? "Пациент успешно добавлен." : "Данные пациента успешно обновлены.";
                MessageBox.Show(successMessage, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OpenPatientDetails(object? obj)
        {
            if (SelectedPatient == null) return;
            var patientDetailsViewModel = new PatientDetailsViewModel(_serviceProvider, SelectedPatient, _currentUser);
            _dialogService.ShowDialog(patientDetailsViewModel);
        }
    }
}