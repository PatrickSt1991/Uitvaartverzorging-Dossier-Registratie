using System;
using System.Windows.Input;

namespace Dossier_Registratie.ViewModels
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _executeAction;

        public DelegateCommand(Action<object> executeAction)
        {
            _executeAction = executeAction;
        }
        public bool CanExecute(object parameter) => true;
        public void Execute(object? parameter) => _executeAction(parameter);

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
