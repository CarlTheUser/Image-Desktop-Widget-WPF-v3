using System;
using System.Windows.Media;

namespace Presentation.Models
{
    public class PinnedImage : BindObservable, IOriginator<Data.Projections.PinnedImage>
    {
        private Color _color;
        private Shared.FrameThickness _frameThickness;
        private Shared.Rotation _rotation;
        private Shared.Corner _corner;

        public Shared.ImageId Id { get; }
        public Shared.ImageDirectory Directory { get; }
        public Dimension Dimension { get; }
        public Location Location { get; }
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged(nameof(Color));
            }
        }
        public Shared.FrameThickness FrameThickness
        {
            get => _frameThickness;
            set
            {
                _frameThickness = value;
                OnPropertyChanged(nameof(FrameThickness));
            }
        }
        public Shared.Rotation Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                OnPropertyChanged(nameof(Rotation));
            }
        }
        public Shared.Corner Corner
        {
            get => _corner;
            set
            {
                _corner = value;
                OnPropertyChanged(nameof(Corner));
            }
        }
        public Caption Caption { get; }
        public Shadow Shadow { get; }
        public DateTime CreationTimestamp { get; }

        public PinnedImage(
            Shared.ImageId id,
            Shared.ImageDirectory directory,
            Dimension dimension,
            Location location,
            Color color,
            Shared.FrameThickness frameThickness,
            Shared.Rotation rotation,
            Shared.Corner corner,
            Caption caption,
            Shadow shadow,
            DateTime creationTimestamp)
        {
            Id = id;
            Directory = directory;
            Dimension = dimension;
            Location = location;
            _color = color;
            _frameThickness = frameThickness;
            _rotation = rotation;
            _corner = corner;
            Caption = caption;
            Shadow = shadow;
            CreationTimestamp = creationTimestamp;
        }

        public Data.Projections.PinnedImage CreateMemento()
        {
            return new Data.Projections.PinnedImage(
                Id: Id,
                Directory: Directory,
                Dimension: Dimension.CreateMemento(),
                Location: Location.CreateMemento(),
                Color: new Shared.ImageColor(HexValue: Color.ToString()),
                FrameThickness: FrameThickness,
                Rotaion: Rotation,
                Corner: Corner,
                Caption: Caption.CreateMemento(),
                Shadow: Shadow.CreateMemento(),
                IsShown: true,
                CreationTimestamp: CreationTimestamp);
        }

        public void Restore(Data.Projections.PinnedImage memento)
        {
            Dimension.Restore(memento.Dimension);
            Location.Restore(memento.Location);
            Color = (Color)ColorConverter.ConvertFromString(value: memento.Color.HexValue);
            FrameThickness = memento.FrameThickness;
            Rotation = memento.Rotaion;
            Corner = memento.Corner;
            Caption.Restore(memento.Caption);
            Shadow.Restore(memento.Shadow);
        }
    }
}
