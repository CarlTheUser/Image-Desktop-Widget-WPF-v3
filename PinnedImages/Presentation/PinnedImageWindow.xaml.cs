using Application.Services;
using Microsoft.Extensions.Logging;
using Presentation.ViewModels;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for PinnedImageWindow.xaml
    /// </summary>
    public partial class PinnedImageWindow : Window, IApplicationViewComponent<PinnedImageViewModel>, IDisplayHost
    {
        public PinnedImageViewModel ViewModel { get; }

        public PinnedImageWindow(
            Models.PinnedImage pinnedImage,
            MainWindowViewLauncher mainWindowViewLauncher,
            IUserNotification<Exception> errorNotification,
            IDeletePinnedImageService deletePinnedImageService,
            IChangePinnedImageDisplayParameterService changePinnedImageDisplayParameterService,
            IUnpinImageService unpinImageService,
            ILogger<PinnedImageViewModel> logger,
            IPinnedImageRestyleViewLauncher pinnedImageRestyleViewLauncher)
        {
            InitializeComponent();

            ViewModel = new(
                logger: logger,
                pinnedImage: pinnedImage,
                pinnedImageDisplayHost: this,
                mainWindowViewLauncher: mainWindowViewLauncher,
                errorNotification: errorNotification,
                deletePinnedImageService: deletePinnedImageService,
                changePinnedImageDisplayParameterService: changePinnedImageDisplayParameterService,
                unpinImageService: unpinImageService,
                pinnedImageRestyleViewLauncher: pinnedImageRestyleViewLauncher);

            DataContext = ViewModel;
        }

        public PinnedImageViewModel GetViewModel()
        {
            return ViewModel;
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private async void Window_Closed(object sender, EventArgs e)
        {
            await ViewModel.SaveStateAsync(cancellationToken: CancellationToken.None);
        }
    }
}
