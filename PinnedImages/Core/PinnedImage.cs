using Core.Events;
using Data.Common.Contracts;
using Shared;

namespace Core
{
    public class PinnedImage : IDataEventSource
    {
        public static PinnedImage New(
            ImageDirectory imageDirectory,
            Dimension dimension,
            Location location,
            ImageColor color,
            FrameThickness frameThickness,
            Rotation rotation,
            Corner corner,
            Caption caption,
            Shadow shadow)
        {
            var pinnedImage = new PinnedImage(
                id: ImageId.New(),
                directory: imageDirectory,
                dimension: dimension,
                location: location,
                color: color,
                frameThickness: frameThickness,
                rotation: rotation,
                corner: corner,
                caption: caption,
                shadow: shadow,
                isPinned: true,
                creationTimestamp: DateTime.Now);

            pinnedImage._events.Enqueue(
                new NewImagePinnedDataEvent(
                    imageId: pinnedImage.Id,
                    directory: pinnedImage.Directory,
                    dimension: pinnedImage.Dimension,
                    location: pinnedImage.Location,
                    color: pinnedImage.Color,
                    frameThickness: pinnedImage.FrameThickness,
                    rotation: pinnedImage.Rotation,
                    corner: pinnedImage.Corner,
                    caption: pinnedImage.Caption,
                    shadow: pinnedImage.Shadow,
                    creationTimestamp: pinnedImage.CreationTimestamp));

            return pinnedImage;
        }

        public static PinnedImage Existing(
            ImageId id,
            ImageDirectory directory,
            Dimension dimension,
            Location location,
            ImageColor color,
            FrameThickness frameThickness,
            Rotation rotation,
            Corner corner,
            Caption caption,
            Shadow shadow,
            bool isPinned,
            DateTime creationTimestamp)
        {
            return new PinnedImage(
                id: id,
                directory: directory,
                dimension: dimension,
                location: location,
                color: color,
                frameThickness: frameThickness,
                rotation: rotation,
                corner: corner,
                caption: caption,
                shadow: shadow,
                isPinned: isPinned,
                creationTimestamp: creationTimestamp);
        }

        private readonly Queue<DataEvent> _events = new();

        public ImageId Id { get; }
        public ImageDirectory Directory { get; }
        public Dimension Dimension { get; private set; }
        public Location Location { get; private set; }
        public ImageColor Color { get; private set; }
        public FrameThickness FrameThickness { get; private set; }
        public Rotation Rotation { get; private set; }
        public Corner Corner { get; private set; }
        public Caption Caption { get; private set; }
        public Shadow Shadow { get; private set; }
        public bool IsPinned { get; private set; }
        public DateTime CreationTimestamp { get; }

        private PinnedImage(ImageId id,
            ImageDirectory directory,
            Dimension dimension,
            Location location,
            ImageColor color,
            FrameThickness frameThickness,
            Rotation rotation,
            Corner corner,
            Caption caption,
            Shadow shadow,
            bool isPinned,
            DateTime creationTimestamp)
        {
            Id = id;
            Directory = directory;
            Dimension = dimension;
            Location = location;
            Color = color;
            FrameThickness = frameThickness;
            Rotation = rotation;
            Corner = corner;
            Caption = caption;
            Shadow = shadow;
            IsPinned = isPinned;
            CreationTimestamp = creationTimestamp;
        }

        public void ApplyVisualStyle(VisualStyle style)
        {
            Color = style.Color;
            FrameThickness = style.FrameThickness;
            Rotation = style.Rotaion;
            Corner = style.Corner;
            Caption = style.Caption;
            Shadow = style.Shadow;

            _events.Enqueue(new ImageRestyledDataEvent(
                imageId: Id,
                visualStyle: style));
        }

        public void ApplyParameters(DisplayParameter parameter)
        {
            Dimension = parameter.Dimension;
            Location = parameter.Location;

            _events.Enqueue(new ImageDisplayParameterChangedDataEvent(
                imageId: Id,
                displayParameter: parameter));
        }

        public void Pin()
        {
            if (IsPinned)
            {
                throw new DomainLogicException("Image is already pinned.");
            }

            IsPinned = true;

            _events.Enqueue(new ImageRepinnedDataEvent(imageId: Id));
        }

        public void UnPin()
        {
            if (!IsPinned)
            {
                throw new DomainLogicException("Image is already unpinned.");
            }

            IsPinned = false;

            _events.Enqueue(new ImageUnpinnedDataEvent(imageId: Id));
        }

        public void Remove()
        {
            _events.Enqueue(new ImageRemovedDataEvent(imageId: Id));
        }

        public IEnumerable<DataEvent> ReleaseEvents()
        {
            IReadOnlyList<DataEvent> copy = _events.ToList();

            _events.Clear();

            return copy;
        }
    }
}
