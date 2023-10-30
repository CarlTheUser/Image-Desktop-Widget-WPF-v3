namespace Presentation
{
    public class MainWindowViewLauncher : IViewLauncher
    {
        private readonly object _lock = new();
        private volatile MainWindow? _mainWindow = null;

        public void Launch()
        {
            if (_mainWindow == null)
            {
                lock (_lock)
                {
                    if (_mainWindow == null)
                    {
                        _mainWindow = new();
                        _mainWindow.Closed += MainWindow_Closed;
                    }
                }
            }

            _mainWindow.Show();
            _mainWindow.Activate();
        }

        private void MainWindow_Closed(object? sender, System.EventArgs e)
        {
            lock (_lock)
            {
                if (_mainWindow != null)
                {
                    _mainWindow.Closed -= MainWindow_Closed;
                }
                _mainWindow = null;
            }
        }
    }
}
