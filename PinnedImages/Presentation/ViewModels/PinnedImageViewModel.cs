using Application.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Presentation.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Presentation.ViewModels
{
    public class PinnedImageViewModel : ViewModelBase
    {
        private readonly ILogger<PinnedImageViewModel> _logger;
        private readonly IDisplayHost _pinnedImageDisplayHost;
        private readonly MainWindowViewLauncher _mainWindowViewLauncher;
        private readonly IUserNotification<Exception> _errorNotification;
        private readonly IDeletePinnedImageService _deletePinnedImageService;
        private readonly IChangePinnedImageDisplayParameterService _changePinnedImageDisplayParameterService;
        private readonly IUnpinImageService _unpinImageService;
        private readonly IPinnedImageRestyleViewLauncher _pinnedImageRestyleViewLauncher;

        private bool _isDeleted = false;

        public PinnedImage Image { get; set; }

        public IRelayCommand ShowHomeCommand { get; }
        public IAsyncRelayCommand DeleteCommand { get; }
        public IAsyncRelayCommand UnPinCommand { get; }
        public IRelayCommand OpenSettingsCommand { get; }

        public PinnedImageViewModel(
            ILogger<PinnedImageViewModel> logger,
            PinnedImage pinnedImage,
            IDisplayHost pinnedImageDisplayHost,
            MainWindowViewLauncher mainWindowViewLauncher,
            IUserNotification<Exception> errorNotification,
            IDeletePinnedImageService deletePinnedImageService,
            IChangePinnedImageDisplayParameterService changePinnedImageDisplayParameterService,
            IUnpinImageService unpinImageService,
            IPinnedImageRestyleViewLauncher pinnedImageRestyleViewLauncher)
        {
            _logger = logger;
            Image = pinnedImage;
            _pinnedImageDisplayHost = pinnedImageDisplayHost;
            _mainWindowViewLauncher = mainWindowViewLauncher;
            _errorNotification = errorNotification;
            _deletePinnedImageService = deletePinnedImageService;
            _changePinnedImageDisplayParameterService = changePinnedImageDisplayParameterService;
            _unpinImageService = unpinImageService;
            _pinnedImageRestyleViewLauncher = pinnedImageRestyleViewLauncher;

            ShowHomeCommand = new RelayCommand(execute: ShowHome);
            OpenSettingsCommand = new RelayCommand(execute: OpenSettings);
            DeleteCommand = new AsyncRelayCommand(execute: Delete);
            UnPinCommand = new AsyncRelayCommand(execute: UnPin);
        }

        private void ShowHome()
        {
            _mainWindowViewLauncher.Launch();
        }

        private void OpenSettings()
        {
            _pinnedImageRestyleViewLauncher.Launch(parameter: Image);
        }

        private async Task Delete()
        {
            if (!_isDeleted)
            {
                try
                {
                    await _deletePinnedImageService.DeletePinnedImage(
                    imageId: Image.Id);

                    _isDeleted = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(exception: ex, message: "An error occurred");
                    _errorNotification.Notify(ex);
                }
                finally
                {
                    _pinnedImageDisplayHost.Close();
                }
            }
        }

        private async Task UnPin()
        {
            try
            {
                await _unpinImageService.Unpin(imageId: Image.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(exception: ex, message: "An error occurred");
                _errorNotification.Notify(ex);
            }
            finally
            {
                _pinnedImageDisplayHost.Close();
            }
        }

        public async Task SaveStateAsync(CancellationToken cancellationToken = default)
        {
            if (!_isDeleted)
            {
                try
                {
                    await _changePinnedImageDisplayParameterService.Apply(
                        imageId: Image.Id, 
                        displayParameter: new Shared.DisplayParameter(
                            Dimension: Image.Dimension.CreateMemento(),
                            Location: Image.Location.CreateMemento()),
                        cancellationToken: cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(exception: ex, message: "An error occurred");
                    _errorNotification.Notify(ex);
                }
            }
        }
    }
}
