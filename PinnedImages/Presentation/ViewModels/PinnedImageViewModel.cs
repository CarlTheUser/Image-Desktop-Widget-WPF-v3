using Application.Services;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FluentResults;
using Microsoft.Extensions.Logging;
using Presentation.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Presentation.ViewModels
{
    public class PinnedImageViewModel : ViewModelBase
    {
        private readonly ILogger<PinnedImageViewModel> _logger;
        private readonly MainWindowViewLauncher _mainWindowViewLauncher;
        private readonly IUserNotification<Exception> _errorNotification;
        private readonly IUserNotification<Presentation.Message> _messageNotification;
        private readonly IDeletePinnedImageService _deletePinnedImageService;
        private readonly IChangePinnedImageDisplayParameterService _changePinnedImageDisplayParameterService;
        private readonly IUnpinImageService _unpinImageService;
        private readonly IPinnedImageRestyleViewLauncher _pinnedImageRestyleViewLauncher;
        private readonly IMessenger _messenger;

        private bool _isDeleted = false;

        public PinnedImage Image { get; set; }

        public IRelayCommand ShowHomeCommand { get; }
        public IAsyncRelayCommand DeleteCommand { get; }
        public IAsyncRelayCommand UnPinCommand { get; }
        public IRelayCommand OpenSettingsCommand { get; }

        public PinnedImageViewModel(
            ILogger<PinnedImageViewModel> logger,
            PinnedImage pinnedImage,
            MainWindowViewLauncher mainWindowViewLauncher,
            IUserNotification<Exception> errorNotification,
            IUserNotification<Presentation.Message> messageNotification,
            IDeletePinnedImageService deletePinnedImageService,
            IChangePinnedImageDisplayParameterService changePinnedImageDisplayParameterService,
            IUnpinImageService unpinImageService,
            IPinnedImageRestyleViewLauncher pinnedImageRestyleViewLauncher,
            IMessenger messenger)
        {
            _logger = logger;
            Image = pinnedImage;
            _mainWindowViewLauncher = mainWindowViewLauncher;
            _errorNotification = errorNotification;
            _messageNotification = messageNotification;
            _deletePinnedImageService = deletePinnedImageService;
            _changePinnedImageDisplayParameterService = changePinnedImageDisplayParameterService;
            _unpinImageService = unpinImageService;
            _pinnedImageRestyleViewLauncher = pinnedImageRestyleViewLauncher;

            ShowHomeCommand = new RelayCommand(execute: ShowHome);
            OpenSettingsCommand = new RelayCommand(execute: OpenSettings);
            DeleteCommand = new AsyncRelayCommand(cancelableExecute: DeleteAsync);
            UnPinCommand = new AsyncRelayCommand(cancelableExecute: UnPinAsync);
            _messenger = messenger;
        }

        private void ShowHome()
        {
            _mainWindowViewLauncher.Launch();
        }

        private void OpenSettings()
        {
            _pinnedImageRestyleViewLauncher.Launch(parameter: Image);
        }

        private async Task DeleteAsync(CancellationToken cancellationToken = default)
        {
            if (!_isDeleted)
            {
                try
                {
                    Result result = await _deletePinnedImageService.DeletePinnedImage(
                        imageId: Image.Id,
                        cancellationToken: cancellationToken);

                    if (result.IsFailed)
                    {
                        _messageNotification.Notify(parameter: new Message(
                            Title: "Oops",
                            Value: result.Errors.FirstOrDefault()?.Message ?? "The unknown happened!"));
                    }

                    _isDeleted = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(exception: ex, message: "An error occurred");
                    _errorNotification.Notify(ex);
                }
                finally
                {
                    _messenger.Send(message: new Messages.PinnedImageDeletedMessage(ImageId: Image.Id));
                }
            }
        }

        private async Task UnPinAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                Result result = await _unpinImageService.Unpin(
                    imageId: Image.Id,
                    cancellationToken: cancellationToken);

                if (result.IsFailed)
                {
                    _messageNotification.Notify(parameter: new Message(
                        Title: "Oops",
                        Value: result.Errors.FirstOrDefault()?.Message ?? "The unknown happened!"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(exception: ex, message: "An error occurred");
                _errorNotification.Notify(ex);
            }
            finally
            {
                _messenger.Send(message: new Messages.PinnedImageUnpinnedMessage(ImageId: Image.Id));
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
