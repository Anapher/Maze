using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Orcus.Administration.ViewModels.Utilities
{
    /// <summary>
    ///     A command whose sole purpose is to relay its functionality to other objects by invoking delegates. The default
    ///     return value for the CanExecute method is 'true' when the command is not executing.
    /// </summary>
    public class AsyncRelayCommand<T> : ICommand
    {
        public delegate Task ExecuteDelegate(T parameter);

        private readonly Func<bool> _canExecute;

        private readonly ExecuteDelegate _execute;
        private bool _isRunning;

        public AsyncRelayCommand(ExecuteDelegate execute) : this(execute, null)
        {
        }

        public AsyncRelayCommand(ExecuteDelegate execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return !_isRunning && (_canExecute == null || _canExecute());
        }

        public async void Execute(object parameter)
        {
            _isRunning = true;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);

            await _execute((T) parameter);

            _isRunning = false;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler CanExecuteChanged;
    }
}