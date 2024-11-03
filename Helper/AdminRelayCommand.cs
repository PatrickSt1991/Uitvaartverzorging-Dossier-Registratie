using System;
using System.Windows.Input;

namespace Dossier_Registratie.Helper
{
    public class AdminRelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Action<object> _executeWithParameter;
        private readonly Func<bool> _canExecute;

        // Constructor for parameterless actions
        public AdminRelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Constructor for parameterized actions
        public AdminRelayCommand(Action<object> executeWithParameter, Func<bool> canExecute = null)
        {
            _executeWithParameter = executeWithParameter ?? throw new ArgumentNullException(nameof(executeWithParameter));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object parameter)
        {
            if (_execute != null)
            {
                _execute();
            }
            else if (_executeWithParameter != null)
            {
                _executeWithParameter(parameter);
            }
        }
    }
}
