using System;
using System.Windows;
using System.Windows.Input;

namespace Presentation.Commands
{
    public class MaximizeRestoreWindowCommand : ICommand
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
            if (parameter != null && parameter is Window window)
            {
                switch (window.WindowState)
                {
                    case WindowState.Maximized:
                        window.WindowState = WindowState.Normal;
                        break;
                    case WindowState.Normal:
                        window.WindowState = WindowState.Maximized;
                        break;
                }
            }
        }
    }
}
