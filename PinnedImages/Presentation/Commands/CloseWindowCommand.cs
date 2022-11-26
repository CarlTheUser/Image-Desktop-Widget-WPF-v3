using System;
using System.Windows;
using System.Windows.Input;

namespace Presentation.Commands
{
    public class CloseWindowCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if(parameter != null && parameter is Window w)
            {
                w.Close();
            }
        }
    }
}
