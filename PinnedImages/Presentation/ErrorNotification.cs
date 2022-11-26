using System;
using System.Windows;

namespace Presentation
{
    public class ErrorNotification : IUserNotification<Exception>
    {
        public void Notify(Exception parameter)
        {
            MessageBox.Show(
                messageBoxText: $"{parameter.Message}\n{parameter.StackTrace}",
                caption: $"An error occurred ({parameter.GetType().Name})");
        }
    }
}
