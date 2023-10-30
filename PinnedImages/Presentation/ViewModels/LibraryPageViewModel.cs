using Application.Services;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Data.Common.Contracts;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Presentation.Messages;
using Presentation.View.Misc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using P = Data.Projections;

namespace Presentation.ViewModels
{
    public class LibraryPageViewModel : ViewModelBase
    {
        private const double _defaultWidth = 300,
            _defaultHeight = 225,
            _defaultLocationX = 50,
            _defaultLocationY = 50,
            _defaultFrameThickness = 12;

        private const string _whiteHexValue = "#FFFFFF"; 

        private readonly IConfiguration _configuration;
        private readonly IAsyncQuery<IEnumerable<P.PinnedImageListItem>> _allPinnedImageListItemsQuery;
        private readonly IAsyncQuery<P.PinnedImage?, Shared.ImageId> _pinnedImageByImageIdQuery;
        private readonly IUserPrompt<Stream?, OpenFileDialogPromptParameter> _openFileDialogPrompt;
        private readonly IPinImageService _pinImageService;
        private readonly IPinnedImageViewLauncher _pinnedImageViewLauncher;
        private readonly IUserNotification<Exception> _errorNotification;
        private readonly IUserNotification<Presentation.Message> _messageNotification;
        private readonly IUnpinImageService _unpinImageService;
        private readonly IRepinImageService _repinImageService;
        private readonly IDeletePinnedImageService _deletePinnedImageService;
        private readonly ILogger<LibraryPageViewModel> _logger;
        private readonly IMessenger _messenger;

        private ObservableCollection<Models.PinnedImageListItem> _pinnedImages = new();
        public IAsyncRelayCommand LoadPinnedImagesCommand { get; }
        public IAsyncRelayCommand PinImageCommand { get; }
        public IAsyncRelayCommand<Models.PinnedImageListItem> TogglePinCommand { get; }
        public IAsyncRelayCommand<Models.PinnedImageListItem> DeleteImageCommand { get; }
        public ObservableCollection<Models.PinnedImageListItem> PinnedImages
        {
            get => _pinnedImages;
            set
            {
                _pinnedImages = value;
                OnPropertyChanged(nameof(PinnedImages));
            }
        }

        public LibraryPageViewModel(
            IConfiguration configuration,
            IAsyncQuery<IEnumerable<P.PinnedImageListItem>> allPinnedImageListItemsQuery,
            IAsyncQuery<P.PinnedImage?, Shared.ImageId> pinnedImageByImageIdQuery,
            IUserPrompt<Stream?, OpenFileDialogPromptParameter> openFileDialogPrompt,
            IPinImageService pinImageService,
            IPinnedImageViewLauncher pinnedImageViewLauncher,
            IUserNotification<Exception> errorNotification,
            IUserNotification<Presentation.Message> messageNotification,
            IUnpinImageService unpinImageService,
            IRepinImageService repinImageService,
            IDeletePinnedImageService deletePinnedImageService,
            ILogger<LibraryPageViewModel> logger,
            IMessenger messenger)
        {
            _configuration = configuration;
            _allPinnedImageListItemsQuery = allPinnedImageListItemsQuery;
            _pinnedImageByImageIdQuery = pinnedImageByImageIdQuery;
            _openFileDialogPrompt = openFileDialogPrompt;
            _pinImageService = pinImageService;
            _pinnedImageViewLauncher = pinnedImageViewLauncher;
            _errorNotification = errorNotification;
            _messageNotification = messageNotification;
            _unpinImageService = unpinImageService;
            _repinImageService = repinImageService;
            _deletePinnedImageService = deletePinnedImageService;
            _logger = logger;
            _messenger = messenger;

            LoadPinnedImagesCommand = new AsyncRelayCommand(cancelableExecute: LoadPinnedImagesAsync);
            PinImageCommand = new AsyncRelayCommand(cancelableExecute: PinImageAsync, canExecute: () => true);
            TogglePinCommand = new AsyncRelayCommand<Models.PinnedImageListItem>(cancelableExecute: TogglePinAsync);
            DeleteImageCommand = new AsyncRelayCommand<Models.PinnedImageListItem>(cancelableExecute: DeleteImageAsync);

            messenger.Register<LibraryPageViewModel, Messages.PinnedImageUnpinnedMessage>(recipient: this, handler: OnPinnedImageUnpinned);
            messenger.Register<LibraryPageViewModel, Messages.PinnedImageDeletedMessage>(recipient: this, handler: OnPinnedImageDeleted);
        }

        private async Task LoadPinnedImagesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                IEnumerable<P.PinnedImageListItem> items = await _allPinnedImageListItemsQuery.ExecuteAsync(cancellationToken);

                //await Task.Delay(5000);

                PinnedImages = new ObservableCollection<Models.PinnedImageListItem>(collection: from item in items
                                                                                                select new Models.PinnedImageListItem(
                                                                                                    id: item.ImageId,
                                                                                                    directory: item.Directory,
                                                                                                    caption: item.Caption.Text,
                                                                                                    isShown: item.IsShown,
                                                                                                    creationTimestamp: item.CreationTimestamp));
            }
            catch (Exception ex)
            {
                _logger.LogError(exception: ex, message: "An error occurred.");
                _errorNotification.Notify(ex);
            }

        }

        private async Task PinImageAsync(CancellationToken cancellationToken = default)
        {
            Stream? stream = _openFileDialogPrompt.Prompt(
                parameter: new OpenFileDialogPromptParameter(
                    Title: "Select Image",
                    Filter: "All supported graphics|*.jpg;*.jpeg;*.png|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|Portable Network Graphic (*.png)|*.png"));

            if (stream == null) return;

            try
            {
                Result<P.PinnedImage> result = await _pinImageService.PinImage(
                    request: new IPinImageService.PinImageRequest(
                        ImageStream: stream,
                        Dimension: new Shared.Dimension(Width: _defaultWidth, Height: _defaultHeight),
                        Location: new Shared.Location(X: _defaultLocationX, Y: _defaultLocationY),
                        Color: new Shared.ImageColor(HexValue: _whiteHexValue),
                        FrameThickness: new Shared.FrameThickness(Value: _defaultFrameThickness),
                        Rotation: Shared.Rotation.Zero,
                        Corner: Shared.Corner.None,
                        Caption: Shared.Caption.None,
                        Shadow: Shared.Shadow.None),
                    cancellationToken: cancellationToken);

                if (result.IsFailed)
                {
                    _messageNotification.Notify(parameter: new Message(
                        Title: "Oops",
                        Value: result.Errors.FirstOrDefault()?.Message ?? "The unknown happened!"));

                    return;
                }

                P.PinnedImage pinnedImage = result.Value;

                pinnedImage.Deconstruct(
                    out Shared.ImageId imageId,
                    out Shared.ImageDirectory imageDirectory,
                    out _,
                    out _,
                    out _,
                    out _,
                    out _,
                    out _,
                    out Shared.Caption caption,
                    out _,
                    out bool isShown,
                    out DateTime creationTimestamp);

                _pinnedImages.Insert(
                    index: 0,
                    item: new Models.PinnedImageListItem(
                        id: imageId,
                        directory: imageDirectory,
                        caption: caption.Text,
                        isShown: isShown,
                        creationTimestamp: creationTimestamp));

                _pinnedImageViewLauncher.Launch(parameter: pinnedImage);
            }
            catch (Exception ex)
            {
                _logger.LogError(exception: ex, message: "An error occurred.");
                _errorNotification.Notify(ex);
            }
            finally
            {
                stream.Dispose();
            }
        }

        private async Task TogglePinAsync(Models.PinnedImageListItem? item, CancellationToken cancellationToken = default)
        {
            if (item == null) return;

            Shared.ImageId imageId = item.Id;

            bool isPinned = item.IsShown;

            try
            {
                Result result;

                if (isPinned)
                {
                    result = await _unpinImageService.Unpin(imageId: imageId);

                    if (result.IsFailed)
                    {
                        _messageNotification.Notify(parameter: new Message(
                            Title: "Oops",
                            Value: result.Errors.FirstOrDefault()?.Message ?? "The unknown happened!"));

                        return;
                    }

                    _messenger.Send(message: new Messages.PinnedImageUnpinnedMessage(ImageId: imageId));
                }
                else
                {
                    result = await _repinImageService.Repin(imageId: imageId);

                    if (result.IsFailed)
                    {
                        _messageNotification.Notify(parameter: new Message(
                            Title: "Oops",
                            Value: result.Errors.FirstOrDefault()?.Message ?? "The unknown happened!"));

                        return;
                    }

                    P.PinnedImage? pinnedImage = await _pinnedImageByImageIdQuery.ExecuteAsync(parameter: imageId);

                    if (pinnedImage != null)
                    {
                        _pinnedImageViewLauncher.Launch(parameter: pinnedImage);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(exception: ex, message: "An error occurred.");

                _errorNotification.Notify(ex);
            }
            finally
            {
                item.IsShown = !isPinned;
            }
        }

        private async Task DeleteImageAsync(Models.PinnedImageListItem? item, CancellationToken cancellationToken = default)
        {
            if (item == null) return;

            Shared.ImageId imageId = item.Id;

            try
            {
                Result result = await _deletePinnedImageService.DeletePinnedImage(imageId: imageId, cancellationToken);

                if (result.IsFailed)
                {
                    _messageNotification.Notify(parameter: new Message(
                        Title: "Oops",
                        Value: result.Errors.FirstOrDefault()?.Message ?? "The unknown happened!"));

                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(exception: ex, message: "An error occurred.");
                _errorNotification.Notify(ex);
            }
            finally
            {
                _messenger.Send(message: new Messages.PinnedImageDeletedMessage(ImageId: imageId));
            }
        }

        public void OnPinnedImageUnpinned(LibraryPageViewModel _, PinnedImageUnpinnedMessage message)
        {
            _pinnedImages.First(x => x.Id == message.ImageId).IsShown = false;
        }

        public void OnPinnedImageDeleted(LibraryPageViewModel _, PinnedImageDeletedMessage message)
        {
            _pinnedImages.Remove(item: _pinnedImages.First(x => x.Id == message.ImageId));
        }
    }
}
