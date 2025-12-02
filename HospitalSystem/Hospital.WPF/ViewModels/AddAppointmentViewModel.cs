using Hospital.Business.Models.Medical;
using Hospital.WPF.Commands;
using Hospital.WPF.ViewModels.AppointmentDetails;
using System.Windows.Input;

namespace Hospital.WPF.ViewModels
{
    /// <summary>
    /// ViewModel для диалогового окна добавления нового медицинского назначения.
    /// </summary>
    public class AddAppointmentViewModel : BaseViewModel
    {
        private BaseViewModel? _currentDetailsViewModel;
        /// <summary>
        /// Текущая ViewModel для отображения специфичных полей выбранного типа назначения.
        /// </summary>
        public BaseViewModel? CurrentDetailsViewModel
        {
            get => _currentDetailsViewModel;
            set
            {
                _currentDetailsViewModel = value;
                OnPropertyChanged();
                // Уведомляем команду "Создать", что условия ее выполнения могли измениться.
                CreateAppointmentCommand.RaiseCanExecuteChanged();
            }
        }

        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// Свойство для хранения созданного назначения после успешного завершения.
        /// </summary>
        public Appointment? CreatedAppointment { get; private set; }

        /// <summary>
        /// Событие для связи с View. Позволяет ViewModel запросить закрытие окна.
        /// </summary>
        public event Action<bool>? CloseRequested;

        public ICommand SelectPillTypeCommand { get; }
        public ICommand SelectInjectionTypeCommand { get; }
        public ICommand SelectDiagnosticTypeCommand { get; }
        public ICommand SelectProphylacticTypeCommand { get; }
        public RelayCommand CreateAppointmentCommand { get; }
        public ICommand CancelCommand { get; }

        public AddAppointmentViewModel()
        {
            // Команды для смены типа назначения (меняют CurrentDetailsViewModel)
            SelectPillTypeCommand = new RelayCommand(_ => CurrentDetailsViewModel = new PillAppointmentDetailsViewModel());
            SelectInjectionTypeCommand = new RelayCommand(_ => CurrentDetailsViewModel = new InjectionAppointmentDetailsViewModel());
            SelectDiagnosticTypeCommand = new RelayCommand(_ => CurrentDetailsViewModel = new DiagnosticAppointmentDetailsViewModel());
            SelectProphylacticTypeCommand = new RelayCommand(_ => CurrentDetailsViewModel = new ProphylacticAppointmentDetailsViewModel());

            CreateAppointmentCommand = new RelayCommand(CreateAppointment, CanCreateAppointment);
            CancelCommand = new RelayCommand(Cancel);

            // По умолчанию показываем форму для таблеток для лучшего UX.
            CurrentDetailsViewModel = new PillAppointmentDetailsViewModel();
        }

        private bool CanCreateAppointment(object? obj)
        {
            // Используем сопоставление с образцом для проверки валидности
            // в зависимости от текущего выбранного типа назначения.
            return CurrentDetailsViewModel switch
            {
                PillAppointmentDetailsViewModel p => !string.IsNullOrWhiteSpace(p.MedicationName),
                InjectionAppointmentDetailsViewModel i => !string.IsNullOrWhiteSpace(i.MedicationName),
                DiagnosticAppointmentDetailsViewModel d => !string.IsNullOrWhiteSpace(d.ProcedureName),
                ProphylacticAppointmentDetailsViewModel p => !string.IsNullOrWhiteSpace(p.ProcedureName),
                _ => false // Если ViewModel не выбрана, создать нельзя.
            };
        }

        private void CreateAppointment(object? obj)
        {
            // Используем сопоставление с образцом для создания объекта
            // правильного типа Appointment на основе данных из CurrentDetailsViewModel.
            CreatedAppointment = CurrentDetailsViewModel switch
            {
                PillAppointmentDetailsViewModel p => new PillAppointment { Notes = Notes, MedicationName = p.MedicationName, Dosage = p.Dosage, Frequency = p.Frequency, Days = p.Days },
                InjectionAppointmentDetailsViewModel i => new InjectionAppointment { Notes = Notes, MedicationName = i.MedicationName, Dosage = i.Dosage, Quantity = i.Quantity },
                DiagnosticAppointmentDetailsViewModel d => new DiagnosticAppointment { Notes = Notes, ProcedureName = d.ProcedureName },
                ProphylacticAppointmentDetailsViewModel p => new ProphylacticAppointment { Notes = Notes, ProcedureName = p.ProcedureName },
                _ => null
            };

            if (CreatedAppointment != null)
            {
                // Сигнализируем View, что работа успешно завершена.
                CloseRequested?.Invoke(true);
            }
        }

        private void Cancel(object? obj)
        {
            // Сигнализируем View, что пользователь отменил операцию.
            CloseRequested?.Invoke(false);
        }
    }
}