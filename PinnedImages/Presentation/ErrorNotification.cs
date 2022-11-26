using System;

namespace Presentation
{
    public class ErrorNotification : IUserNotification<Exception>
    {
        public void Notify(Exception parameter)
        {
            new ErrorDisplayWindow(exception: parameter).ShowDialog();
        }
    }
}
