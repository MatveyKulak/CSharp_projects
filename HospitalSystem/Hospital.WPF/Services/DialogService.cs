using Hospital.WPF.ViewModels;
using Hospital.WPF.Views;
using System.Windows;

namespace Hospital.WPF.Services
{
    /// <summary>
    /// Реализует сервис для отображения диалоговых окон, связывая ViewModel с их View.
    /// </summary>
    public class DialogService : IDialogService
    {
        // Словарь для сопоставления типов ViewModel с типами View.
        private readonly Dictionary<Type, Type> _mappings = new();

        public DialogService()
        {
            // Регистрация пар "ViewModel -> View" при создании сервиса.
            _mappings.Add(typeof(PatientEditorViewModel), typeof(PatientEditorView));
            _mappings.Add(typeof(AddAppointmentViewModel), typeof(AddAppointmentView));
            _mappings.Add(typeof(PatientDetailsViewModel), typeof(PatientDetailsView));
            _mappings.Add(typeof(UserEditorViewModel), typeof(UserEditorView));
        }

        public bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : class
        {
            // 1. Находим тип View по типу ViewModel в словаре.
            if (!_mappings.TryGetValue(typeof(TViewModel), out var viewType))
            {
                throw new KeyNotFoundException($"Для ViewModel типа '{typeof(TViewModel).Name}' не зарегистрировано View.");
            }

            // 2. Создаем экземпляр окна, используя рефлексию.
            //    Предполагается, что у View есть конструктор, принимающий ViewModel.
            var window = (Window)Activator.CreateInstance(viewType, viewModel)!;

            // 3. Отображаем окно как модальный диалог и возвращаем результат.
            return window.ShowDialog();
        }
    }
}