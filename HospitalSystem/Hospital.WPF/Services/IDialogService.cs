namespace Hospital.WPF.Services
{
    /// <summary>
    /// Определяет контракт для сервиса, управляющего отображением диалоговых окон.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Отображает диалоговое окно для заданной ViewModel.
        /// </summary>
        /// <typeparam name="TViewModel">Тип ViewModel, для которой ищется View.</typeparam>
        /// <returns>Результат диалога (true, false или null).</returns>
        bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : class;
    }
}