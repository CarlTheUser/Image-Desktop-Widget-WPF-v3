namespace Presentation
{
    public class MainWindowViewLauncher : IViewLauncher
    {
        private readonly object _lock = new();
        private readonly object _releaseLock = new();

        private MainWindow? _mainWindow = null;
        public void Launch()
        {
            if (_mainWindow == null)
            {
                lock (_lock)
                {
                    if (_mainWindow == null)
                    {
                        _mainWindow = new();

                        _mainWindow.Closed += _mainWindow_Closed;
                    }
                }
            }

            _mainWindow.Show();
            _mainWindow.Activate();
        }

        private void _mainWindow_Closed(object? sender, System.EventArgs e)
        {
            lock (_releaseLock)
            {
                if (_mainWindow != null) _mainWindow.Closed -= _mainWindow_Closed;
                _mainWindow = null;
            }
        }
    }
}
