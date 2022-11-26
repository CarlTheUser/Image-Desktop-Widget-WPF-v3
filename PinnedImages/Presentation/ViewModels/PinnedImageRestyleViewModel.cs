using Application.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using P = Data.Projections;
namespace Presentation.ViewModels
{
    public class PinnedImageRestyleViewModel : ViewModelBase
    {
        private readonly ILogger<PinnedImageRestyleViewModel> _logger;
        private readonly IRestylePinnedImageService _restylePinnedImageService;
        private readonly IUserNotification<Exception> _errorNotification;
        private readonly IDisplayHost _displayHost;
        private P.PinnedImage _state;

        public Models.PinnedImage PinnedImage { get; }

        public ObservableCollection<PresetSetting<Shared.FrameThickness>> FrameThicknessPresets { get; } = new ObservableCollection<PresetSetting<Shared.FrameThickness>>()
        {
            new PresetSetting<Shared.FrameThickness>(
                Description: "None",
                Value: new Shared.FrameThickness(Value: 0)),
            new PresetSetting<Shared.FrameThickness>(
                Description: "Thin",
                Value: new Shared.FrameThickness(Value: 6)),
            new PresetSetting<Shared.FrameThickness>(
                Description: "Normal",
                Value: new Shared.FrameThickness(Value: 12)),
            new PresetSetting<Shared.FrameThickness>(
                Description: "Thick",
                Value: new Shared.FrameThickness(Value: 16)),
            new PresetSetting<Shared.FrameThickness>(
                Description: "Extra Thick",
                Value: new Shared.FrameThickness(Value: 20))
        };

        public ObservableCollection<PresetSetting<Shared.Rotation>> RotationPresets { get; } = new ObservableCollection<PresetSetting<Shared.Rotation>>()
        {
            new PresetSetting<Shared.Rotation>(
                Description: "0°",
                Value: new Shared.Rotation(Angle: 0)),
            new PresetSetting<Shared.Rotation>(
                Description: "15°",
                Value: new Shared.Rotation(Angle: 15)),
            new PresetSetting<Shared.Rotation>(
                Description: "30°",
                Value: new Shared.Rotation(Angle: 30)),
            new PresetSetting<Shared.Rotation>(
                Description: "45°",
                Value: new Shared.Rotation(Angle: 45)),
            new PresetSetting<Shared.Rotation>(
                Description: "60°",
                Value: new Shared.Rotation(Angle: 60)),
            new PresetSetting<Shared.Rotation>(
                Description: "90°",
                Value: new Shared.Rotation(Angle: 90)),
        };

        public ObservableCollection<PresetSetting<Shared.Shadow>> ShadowPresets { get; } = new ObservableCollection<PresetSetting<Shared.Shadow>>()
        {
            new PresetSetting<Shared.Shadow>(
                Description: "None",
                Value: new Shared.Shadow(
                    Opacity: 0,
                    Depth: 0,
                    Direction: 0,
                    BlurRadius: 0,
                    IsVisible: false)),
             new PresetSetting<Shared.Shadow>(
                Description: "Small",
                Value: new Shared.Shadow(
                    Opacity: 0.05,
                    Depth: 1,
                    Direction: 270,
                    BlurRadius: 2,
                    IsVisible: false)),
             new PresetSetting<Shared.Shadow>(
                Description: "Normal",
                Value: new Shared.Shadow(
                    Opacity: 0.1,
                    Depth: 1,
                    Direction: 270,
                    BlurRadius: 3,
                    IsVisible: false)),
             new PresetSetting<Shared.Shadow>(
                Description: "Medium",
                Value: new Shared.Shadow(
                    Opacity: 0.1,
                    Depth: 4,
                    Direction: 270,
                    BlurRadius: 6,
                    IsVisible: false)),
             new PresetSetting<Shared.Shadow>(
                Description: "Large",
                Value: new Shared.Shadow(
                    Opacity: 0.1,
                    Depth: 10,
                    Direction: 270,
                    BlurRadius: 15,
                    IsVisible: false))
        };

        public IAsyncRelayCommand ApplyChangesCommand { get; }

        public IRelayCommand RollbackChangesCommand { get; }

        public PinnedImageRestyleViewModel(
            ILogger<PinnedImageRestyleViewModel> logger,
            IRestylePinnedImageService restylePinnedImageService,
            IUserNotification<Exception> errorNotification,
            IDisplayHost displayHost,
            Models.PinnedImage pinnedImage)
        {
            _logger = logger;
            _restylePinnedImageService = restylePinnedImageService;
            _errorNotification = errorNotification;
            _displayHost = displayHost;
            PinnedImage = pinnedImage;
            _state = pinnedImage.CreateMemento();

            ApplyChangesCommand = new AsyncRelayCommand(execute: ApplyChanges);

            RollbackChangesCommand = new RelayCommand(execute: RollbackChanges);
        }

        private async Task ApplyChanges()
        {
            try
            {
                await _restylePinnedImageService.Apply(
                    imageId: PinnedImage.Id,
                    style: new Shared.VisualStyle(
                        Color: new Shared.ImageColor(HexValue: PinnedImage.Color.ToString()),
                        FrameThickness: PinnedImage.FrameThickness,
                        Rotaion: PinnedImage.Rotation,
                        Corner: PinnedImage.Corner,
                        Caption: PinnedImage.Caption.CreateMemento(),
                        Shadow: PinnedImage.Shadow.CreateMemento()));

                _state = PinnedImage.CreateMemento();

                _displayHost.Close();
            }
            catch(Exception ex)
            {
                _logger.LogError(exception: ex, message: "An error occurred.");

                _errorNotification.Notify(ex);
            }
        }

        public void RollbackChanges()
        {
            PinnedImage.Restore(memento: _state);
            _displayHost.Close();
        }

        public record PresetSetting<T>(
            string Description,
            T Value);
    }
}
