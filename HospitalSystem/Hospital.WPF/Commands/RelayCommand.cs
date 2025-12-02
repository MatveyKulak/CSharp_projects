using System.Windows.Input;

namespace Hospital.WPF.Commands
{
    /// <summary>
    /// Предоставляет универсальную реализацию интерфейса ICommand для синхронных операций.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        /// <summary>
        /// Событие, которое возникает при изменении условий, влияющих на возможность выполнения команды.
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Определяет, может ли команда выполняться в ее текущем состоянии.
        /// </summary>
        public bool CanExecute(object? parameter)
        {
            // Если предикат не задан, команда всегда доступна. Иначе - результат предиката.
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Выполняет логику команды.
        /// </summary>
        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// Позволяет вручную инициировать перепроверку CanExecute.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}