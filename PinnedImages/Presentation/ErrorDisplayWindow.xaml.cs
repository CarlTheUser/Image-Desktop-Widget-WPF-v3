using System;
using System.Windows;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for ErrorDisplayWindow.xaml
    /// </summary>
    public partial class ErrorDisplayWindow : Window
    {
        public Exception Exception { get; }
        public ErrorDisplayWindow(Exception exception)
        {
            InitializeComponent();
            Exception = exception;
            this.DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
