using Hospital.Business.Enums;
using Hospital.Business.Models.Medical;
using Hospital.Business.Models.People;
using Hospital.Services.Interfaces;
using Hospital.WPF.Commands;
using Hospital.WPF.Services;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Windows;

namespace Hospital.WPF.ViewModels
{
    /// <summary>
    /// ViewModel для окна с детальной информацией о пациенте и его назначениях.
    /// </summary>
    public class PatientDetailsViewModel : BaseViewModel
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDialogService _dialogService;
        private readonly User _currentUser;

        public Patient SelectedPatient { get; }

        private Appointment? _selectedAppointment;
        public Appointment? SelectedAppointment
        {
            get => _selectedAppointment;
            set
            {
                if (_selectedAppointment != null)
                    _selectedAppointment.PropertyChanged -= OnAppointmentStatusChanged;

                _selectedAppointment = value;

                if (_selectedAppointment != null)
                    _selectedAppointment.PropertyChanged += OnAppointmentStatusChanged;

                OnPropertyChanged();
                DeleteAppointmentCommand.RaiseCanExecuteChanged();
                CompleteAppointmentCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsNurse => _currentUser is Nurse;
        public bool IsDoctor => _currentUser is Doctor;

        public RelayCommand AddAppointmentCommand { get; }
        public RelayCommand DeleteAppointmentCommand { get; }
        public RelayCommand CompleteAppointmentCommand { get; }

        public PatientDetailsViewModel(IServiceProvider serviceProvider, Patient selectedPatient, User currentUser)
        {
            _serviceProvider = serviceProvider;
            _dialogService = _serviceProvider.GetRequiredService<IDialogService>();
            SelectedPatient = selectedPatient;
            _currentUser = currentUser;

            AddAppointmentCommand = new RelayCommand(AddAppointment, _ => IsDoctor);
            DeleteAppointmentCommand = new RelayCommand(DeleteAppointment, CanDeleteAppointment);
            CompleteAppointmentCommand = new RelayCommand(CompleteAppointment, CanCompleteAppointment);
        }

        /// <summary>
        /// Обработчик события изменения статуса в ComboBox'е (привязанного к Appointment.Status).
        /// </summary>
        private async void OnAppointmentStatusChanged(object? sender, PropertyChangedEventArgs elem)
        {
            if (elem.PropertyName == nameof(Appointment.Status) && SelectedAppointment != null)
            {
                using var scope = _serviceProvider.CreateScope();
                var appointmentService = scope.ServiceProvider.GetRequiredService<IAppointmentService>();
                await appointmentService.UpdateAppointmentAsync(SelectedAppointment);
                CompleteAppointmentCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanDeleteAppointment(object? obj)
        {
            // Удалять может только врач и только если что-то выбрано
            return IsDoctor && SelectedAppointment != null;
        }

        /// <summary>
        /// Определяет, может ли текущий пользователь выполнить выбранное назначение.
        /// Реализует бизнес-логику разделения прав (врач/медсестра).
        /// </summary>
        private bool CanCompleteAppointment(object? obj)
        {
            if (SelectedAppointment == null || SelectedAppointment.Status != AppointmentStatus.Scheduled)
            {
                return false; // Нельзя выполнить, если ничего не выбрано или уже выполнено/отменено
            }

            // Медсестра может выполнять только медикаментозные
            if (_currentUser is Nurse && SelectedAppointment is MedicationAppointment)
            {
                return true;
            }

            // Врач может выполнять остальные (диагностические и профилактические)
            if (_currentUser is Doctor && (SelectedAppointment is DiagnosticAppointment || SelectedAppointment is ProphylacticAppointment))
            {
                return true;
            }

            return false; // Во всех остальных случаях - нельзя
        }

        private async void DeleteAppointment(object? obj)
        {
            if (SelectedAppointment == null) return;
            var result = MessageBox.Show("Вы уверены, что хотите удалить это назначение?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;

            using var scope = _serviceProvider.CreateScope();
            var appointmentService = scope.ServiceProvider.GetRequiredService<IAppointmentService>();
            await appointmentService.DeleteAppointmentAsync(SelectedAppointment.Id);
            SelectedPatient.MedicalRecord.Appointments.Remove(SelectedAppointment);
        }

        private void CompleteAppointment(object? obj)
        {
            if (SelectedAppointment != null)
            {
                // Этот метод просто меняет статус в объекте.
                // Сохранение в БД происходит автоматически благодаря OnAppointmentStatusChanged.
                SelectedAppointment.Status = AppointmentStatus.Completed;
            }
        }

        private void AddAppointment(object? obj)
        {
            if (_currentUser is not Doctor currentDoctor) return;

            var addAppointmentViewModel = new AddAppointmentViewModel();
            var result = _dialogService.ShowDialog(addAppointmentViewModel);

            if (result == true)
            {
                var newAppointment = addAppointmentViewModel.CreatedAppointment;
                if (newAppointment == null) return;
                SaveNewAppointment(newAppointment, currentDoctor.Id);
            }
        }

        private async void SaveNewAppointment(Appointment newAppointment, Guid doctorId)
        {
            using var scope = _serviceProvider.CreateScope();
            var appointmentService = scope.ServiceProvider.GetRequiredService<IAppointmentService>();
            Appointment createdAppointment;

            switch (newAppointment)
            {
                case MedicationAppointment med:
                    createdAppointment = await appointmentService.AddMedicationAppointmentAsync(SelectedPatient, med, doctorId);
                    break;
                case DiagnosticAppointment diag:
                    createdAppointment = await appointmentService.AddDiagnosticAppointmentAsync(SelectedPatient, diag, doctorId);
                    break;
                case ProphylacticAppointment proph:
                    createdAppointment = await appointmentService.AddProphylacticAppointmentAsync(SelectedPatient, proph, doctorId);
                    break;
                default:
                    throw new NotSupportedException($"Тип назначения '{newAppointment.GetType().Name}' не поддерживается.");
            }

            SelectedPatient.MedicalRecord.Appointments.Add(createdAppointment);
        }
    }
}