using Application.Services;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Data.Common.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Presentation.Messages;
using Presentation.View.Misc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using P = Data.Projections;

namespace Presentation.ViewModels
{
    public class LibraryPageViewModel : ViewModelBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAsyncQuery<IEnumerable<P.PinnedImageListItem>> _allPinnedImageListItemsQuery;
        private readonly IUserPrompt<Stream?, OpenFileDialogPromptParameter> _openFileDialogPrompt;
        private readonly IPinImageService _pinImageService;
        private readonly IPinnedImageViewLauncher _pinnedImageViewLauncher;
        private readonly IUserNotification<Exception> _errorNotification;
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
            IUserPrompt<Stream?, OpenFileDialogPromptParameter> openFileDialogPrompt,
            IPinImageService pinImageService,
            IPinnedImageViewLauncher pinnedImageViewLauncher,
            IUserNotification<Exception> errorNotification,
            IUnpinImageService unpinImageService,
            IRepinImageService repinImageService,
            IDeletePinnedImageService deletePinnedImageService,
            ILogger<LibraryPageViewModel> logger,
            IMessenger messenger)
        {
            _configuration = configuration;
            _allPinnedImageListItemsQuery = allPinnedImageListItemsQuery;
            _openFileDialogPrompt = openFileDialogPrompt;
            _pinImageService = pinImageService;
            _pinnedImageViewLauncher = pinnedImageViewLauncher;
            _errorNotification = errorNotification;
            _unpinImageService = unpinImageService;
            _repinImageService = repinImageService;
            _deletePinnedImageService = deletePinnedImageService;
            _logger = logger;
            _messenger = messenger;

            LoadPinnedImagesCommand = new AsyncRelayCommand(execute: LoadPinnedImages);
            PinImageCommand = new AsyncRelayCommand(execute: PinImage, canExecute: () => true);
            TogglePinCommand = new AsyncRelayCommand<Models.PinnedImageListItem>(execute: TogglePin);
            DeleteImageCommand = new AsyncRelayCommand<Models.PinnedImageListItem>(execute: DeleteImage);

            messenger.Register<LibraryPageViewModel, Messages.PinnedImageUnpinnedMessage>(recipient: this, handler: OnPinnedImageUnpinned);
            messenger.Register<LibraryPageViewModel, Messages.PinnedImageDeletedMessage>(recipient: this, handler: OnPinnedImageDeleted);
        }
        private async Task LoadPinnedImages()
        {
            IEnumerable<P.PinnedImageListItem> items = await _allPinnedImageListItemsQuery.ExecuteAsync();

            //await Task.Delay(5000);

            PinnedImages = new ObservableCollection<Models.PinnedImageListItem>(collection: from item in items
                                                                                            select new Models.PinnedImageListItem(
                                                                                                id: item.ImageId,
                                                                                                directory: item.Directory,
                                                                                                caption: item.Caption.Text,
                                                                                                isShown: item.IsShown,
                                                                                                creationTimestamp: item.CreationTimestamp));
        }

        private async Task PinImage()
        {
            Stream? stream = _openFileDialogPrompt.Prompt(
                parameter: new OpenFileDialogPromptParameter(
                    Title: "Select Image",
                    Filter: "All supported graphics|*.jpg;*.jpeg;*.png|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|Portable Network Graphic (*.png)|*.png"));

            if (stream == null) return;

            try
            {
                P.PinnedImage pinnedImage = await _pinImageService.PinImage(
                    request: new IPinImageService.PinImageRequest(
                        ImageStream: stream,
                        Dimension: new Shared.Dimension(Width: 300, Height: 225),
                        Location: new Shared.Location(X: 50, Y: 50),
                        Color: new Shared.ImageColor(HexValue: "#FFFFFF"),
                        FrameThickness: new Shared.FrameThickness(Value: 12),
                        Rotation: new Shared.Rotation(Angle: 0),
                        Corner: Shared.Corner.None,
                        Caption: Shared.Caption.None,
                        Shadow: Shared.Shadow.None));

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

                _pinnedImages.Insert(index: 0, item: new Models.PinnedImageListItem(
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

        private async Task TogglePin(Models.PinnedImageListItem? item)
        {
            if (item == null) return;

            bool isPinned = item.IsShown;

            try
            {
                if (isPinned)
                {
                    await _unpinImageService.Unpin(imageId: item.Id);
                }
                else
                {
                    await _repinImageService.Repin(imageId: item.Id);
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

        private async Task DeleteImage(Models.PinnedImageListItem? item)
        {
            if (item == null) return;

            Shared.ImageId imageId = item.Id;

            try
            {
                await _deletePinnedImageService.DeletePinnedImage(imageId: imageId);
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
