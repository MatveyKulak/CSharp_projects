using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Hospital.WPF.ViewModels
{
    /// <summary>
    /// Базовый класс для всех ViewModel, реализующий интерфейс INotifyPropertyChanged.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Вызывает событие PropertyChanged для уведомления UI об изменении свойства.
        /// </summary>
        /// <param name="propertyName">Имя измененного свойства. Автоматически подставляется компилятором.</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}