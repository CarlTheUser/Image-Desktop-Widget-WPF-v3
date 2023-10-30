using Application.Services;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using P = Data.Projections;
namespace Presentation.ViewModels
{
    public class PinnedImageRestyleViewModel : ViewModelBase
    {
        private readonly ILogger<PinnedImageRestyleViewModel> _logger;
        private readonly IRestylePinnedImageService _restylePinnedImageService;
        private readonly IUserNotification<Exception> _errorNotification;
        private readonly IUserNotification<Presentation.Message> _messageNotification;
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

        public ObservableCollection<PresetSetting<Shared.Corner>> CornerPresets { get; } = new ObservableCollection<PresetSetting<Shared.Corner>>()
        {
            new PresetSetting<Shared.Corner>(
                Description: "None",
                Value: new Shared.Corner(Radius: 0)),
            new PresetSetting<Shared.Corner>(
                Description: "Small",
                Value: new Shared.Corner(Radius: 4)),
            new PresetSetting<Shared.Corner>(
                Description: "Medium",
                Value: new Shared.Corner(Radius: 7.5)),
            new PresetSetting<Shared.Corner>(
                Description: "Large",
                Value: new Shared.Corner(Radius: 12)),
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
                Description: "Subtle",
                Value: new Shared.Shadow(
                    Opacity: 0.25,
                    Depth: 2,
                    Direction: 270,
                    BlurRadius: 5,
                    IsVisible: true)),
             new PresetSetting<Shared.Shadow>(
                Description: "Medium",
                Value: new Shared.Shadow(
                    Opacity: 0.5,
                    Depth: 4,
                    Direction: 270,
                    BlurRadius: 8,
                    IsVisible: true)),
             new PresetSetting<Shared.Shadow>(
                Description: "Heavy",
                Value: new Shared.Shadow(
                    Opacity: 0.4,
                    Depth: 8,
                    Direction: 270,
                    BlurRadius: 10,
                    IsVisible: true)),
             new PresetSetting<Shared.Shadow>(
                Description: "Directional",
                Value: new Shared.Shadow(
                    Opacity: 0.3,
                    Depth: 5,
                    Direction: 315,
                    BlurRadius: 3.68,
                    IsVisible: true))
        };

        public IRelayCommand ApplyFrameThicknessPresetCommand { get; }

        public IRelayCommand ApplyRotationPresetCommand { get; }

        public IRelayCommand ApplyCornerPresetCommand { get; }

        public IRelayCommand ApplyShadowPresetCommand { get; }

        public IAsyncRelayCommand ApplyChangesCommand { get; }

        public IRelayCommand RollbackChangesCommand { get; }

        public PinnedImageRestyleViewModel(
            ILogger<PinnedImageRestyleViewModel> logger,
            IRestylePinnedImageService restylePinnedImageService,
            IUserNotification<Exception> errorNotification,
            IUserNotification<Presentation.Message> messageNotification,
            IDisplayHost displayHost,
            Models.PinnedImage pinnedImage)
        {
            _logger = logger;
            _restylePinnedImageService = restylePinnedImageService;
            _errorNotification = errorNotification;
            _messageNotification = messageNotification;
            _displayHost = displayHost;
            PinnedImage = pinnedImage;
            _state = pinnedImage.CreateMemento();

            ApplyFrameThicknessPresetCommand = new RelayCommand<PresetSetting<Shared.FrameThickness>>(execute: ApplyPreset);
            ApplyRotationPresetCommand = new RelayCommand<PresetSetting<Shared.Rotation>>(execute: ApplyPreset);
            ApplyCornerPresetCommand = new RelayCommand<PresetSetting<Shared.Corner>>(execute: ApplyPreset);
            ApplyShadowPresetCommand = new RelayCommand<PresetSetting<Shared.Shadow>>(execute: ApplyPreset);

            ApplyChangesCommand = new AsyncRelayCommand(cancelableExecute: ApplyChangesAsync);

            RollbackChangesCommand = new RelayCommand(execute: RollbackChanges);
        }

        private void ApplyPreset(PresetSetting<Shared.FrameThickness>? preset)
        {
            if (preset != null)
            {
                PinnedImage.FrameThickness = preset.Value;
            }
        }

        private void ApplyPreset(PresetSetting<Shared.Rotation>? preset)
        {
            if (preset != null)
            {
                PinnedImage.Rotation = preset.Value;
            }
        }

        private void ApplyPreset(PresetSetting<Shared.Corner>? preset)
        {
            if (preset != null)
            {
                PinnedImage.Corner = preset.Value;
            }
        }

        private void ApplyPreset(PresetSetting<Shared.Shadow>? preset)
        {
            if (preset != null)
            {
                (double Opacity,
                    double Depth,
                    double Direction,
                    double BlurRadius,
                    bool IsVisible) = preset.Value;

                PinnedImage.Shadow.Opacity = Opacity;
                PinnedImage.Shadow.Depth = Depth;
                PinnedImage.Shadow.Direction = Direction;
                PinnedImage.Shadow.BlurRadius = BlurRadius;
                PinnedImage.Shadow.Visible = IsVisible;
            }
        }

        private async Task ApplyChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                Result result = await _restylePinnedImageService.Apply(
                    imageId: PinnedImage.Id,
                    style: new Shared.VisualStyle(
                        Color: new Shared.ImageColor(HexValue: PinnedImage.Color.ToString()),
                        FrameThickness: PinnedImage.FrameThickness,
                        Rotation: PinnedImage.Rotation,
                        Corner: PinnedImage.Corner,
                        Caption: PinnedImage.Caption.CreateMemento(),
                        Shadow: PinnedImage.Shadow.CreateMemento()),
                    cancellationToken: cancellationToken);

                if (result.IsFailed)
                {
                    _messageNotification.Notify(parameter: new Message(
                        Title: "Oops",
                        Value: result.Errors.FirstOrDefault()?.Message ?? "The unknown happened!"));

                    return;
                }

                _state = PinnedImage.CreateMemento();

                _displayHost.Close();
            }
            catch (Exception ex)
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
