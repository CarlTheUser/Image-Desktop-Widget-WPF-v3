using Application.Services;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Presentation.Messages;
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
    public partial class PinnedImageWindow : Window,
        IApplicationViewComponent<PinnedImageViewModel>
    {
        private readonly IMessenger _messenger;
        public PinnedImageViewModel ViewModel { get; }
        public PinnedImageWindow(
            Models.PinnedImage pinnedImage,
            MainWindowViewLauncher mainWindowViewLauncher,
            IUserNotification<Exception> errorNotification,
            IUserNotification<Presentation.Message> messageNotification,
            IDeletePinnedImageService deletePinnedImageService,
            IChangePinnedImageDisplayParameterService changePinnedImageDisplayParameterService,
            IUnpinImageService unpinImageService,
            ILogger<PinnedImageViewModel> logger,
            IPinnedImageRestyleViewLauncher pinnedImageRestyleViewLauncher,
            IMessenger messenger)
        {
            InitializeComponent();

            ViewModel = new(
                logger: logger,
                pinnedImage: pinnedImage,
                mainWindowViewLauncher: mainWindowViewLauncher,
                errorNotification: errorNotification,
                messageNotification: messageNotification,
                deletePinnedImageService: deletePinnedImageService,
                changePinnedImageDisplayParameterService: changePinnedImageDisplayParameterService,
                unpinImageService: unpinImageService,
                pinnedImageRestyleViewLauncher: pinnedImageRestyleViewLauncher,
                messenger: messenger);

            DataContext = ViewModel;

            messenger.Register<PinnedImageWindow, Messages.PinnedImageUnpinnedMessage>(recipient: this, handler: OnPinnedImageUnpinned);
            messenger.Register<PinnedImageWindow, Messages.PinnedImageDeletedMessage>(recipient: this, handler: OnPinnedImageDeleted);

            _messenger = messenger;
        }

        public PinnedImageViewModel GetViewModel()
        {
            return ViewModel;
        }

        private void Window_MouseDown(object _, System.Windows.Input.MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private async void Window_Closed(object sender, EventArgs e)
        {
            await ViewModel.SaveStateAsync(cancellationToken: CancellationToken.None);

            _messenger.Unregister<Messages.PinnedImageUnpinnedMessage>(recipient: this);
            _messenger.Unregister<Messages.PinnedImageDeletedMessage>(recipient: this);
        }

        public void OnPinnedImageUnpinned(PinnedImageWindow _, PinnedImageUnpinnedMessage message)
        {
            if (message.ImageId == ViewModel.Image.Id)
            {
                Close();
            }
        }

        public void OnPinnedImageDeleted(PinnedImageWindow _, PinnedImageDeletedMessage message)
        {
            if (message.ImageId == ViewModel.Image.Id)
            {
                Close();
            }
        }
    }
}
