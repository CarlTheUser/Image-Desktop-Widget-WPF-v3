using Application.Services;
using Microsoft.Extensions.Logging;
using Presentation.ViewModels;
using System;
using System.Windows;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for PinnedImageRestyleWindow.xaml
    /// </summary>
    public partial class PinnedImageRestyleWindow : Window, IApplicationViewComponent<PinnedImageRestyleViewModel>, IDisplayHost
    {
        public PinnedImageRestyleViewModel ViewModel { get; }

        public PinnedImageRestyleWindow(
            Models.PinnedImage pinnedImage,
            ILogger<PinnedImageRestyleViewModel> logger,
            IRestylePinnedImageService restylePinnedImageService,
            IUserNotification<Exception> errorNotification,
            IUserNotification<Presentation.Message> messageNotification)
        {
            ViewModel = new PinnedImageRestyleViewModel(
                logger: logger,
                restylePinnedImageService: restylePinnedImageService,
                errorNotification: errorNotification,
                messageNotification: messageNotification,
                displayHost: this,
                pinnedImage: pinnedImage);

            InitializeComponent();

            DataContext = ViewModel;
        }

        public PinnedImageRestyleViewModel GetViewModel()
        {
            return ViewModel;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ViewModel.RollbackChanges();
        }
    }
}
