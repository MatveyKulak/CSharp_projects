using System.Windows.Input;

namespace Hospital.WPF.Commands
{
    /// <summary>
    /// Реализация ICommand для асинхронных операций.
    /// Блокирует повторное выполнение до завершения текущей задачи.
    /// </summary>
    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<object?, Task> _execute;
        private readonly Predicate<object?>? _canExecute;
        private bool _isExecuting;

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public AsyncRelayCommand(Func<object?, Task> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            // Команда доступна, если она не выполняется в данный момент И 
            // если ее предикат (если он есть) возвращает true.
            return !_isExecuting && (_canExecute == null || _canExecute(parameter));
        }

        public async void Execute(object? parameter)
        {
            _isExecuting = true;
            CommandManager.InvalidateRequerySuggested(); // Говорим UI перепроверить CanExecute (кнопка станет неактивной)
            try
            {
                await _execute(parameter);
            }
            finally
            {
                _isExecuting = false;
                CommandManager.InvalidateRequerySuggested(); // Говорим UI снова перепроверить CanExecute (кнопка станет активной)
            }
        }
    }
}