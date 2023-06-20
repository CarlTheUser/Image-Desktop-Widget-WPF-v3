using System.Windows;

namespace Presentation
{
    public record Message(string Title, string Value);

    public class MessageNotification : IUserNotification<Message>
    {
        public void Notify(Message parameter)
        {
            (string title, string message) = parameter;

            MessageBox.Show(
                messageBoxText: message,
                caption: title,
                button: MessageBoxButton.OK);
        }
    }
}
