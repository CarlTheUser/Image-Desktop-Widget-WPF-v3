using Data.Common.Contracts;
using Shared;

namespace Core.Events
{
    public class NewImagePinnedDataEvent : DataEvent
    {
        public ImageId ImageId { get; }
        public ImageDirectory Directory { get; }
        public Dimension Dimension { get; }
        public Location Location { get; }
        public ImageColor Color { get; }
        public FrameThickness FrameThickness { get; }
        public Rotation Rotation { get; }
        public Corner Corner { get; }
        public Caption Caption { get; }
        public Shadow Shadow { get; }
        public DateTime CreationTimestamp { get; }

        public NewImagePinnedDataEvent(ImageId imageId, ImageDirectory directory, Dimension dimension, Location location, ImageColor color, FrameThickness frameThickness, Rotation rotation, Corner corner, Caption caption, Shadow shadow, DateTime creationTimestamp)
        {
            ImageId = imageId;
            Directory = directory;
            Dimension = dimension;
            Location = location;
            Color = color;
            FrameThickness = frameThickness;
            Rotation = rotation;
            Corner = corner;
            Caption = caption;
            Shadow = shadow;
            CreationTimestamp = creationTimestamp;
        }
    }

    public class ImageRestyledDataEvent : DataEvent
    {
        public ImageId ImageId { get; }
        public VisualStyle VisualStyle { get; }

        public ImageRestyledDataEvent(ImageId imageId, VisualStyle visualStyle)
        {
            ImageId = imageId;
            VisualStyle = visualStyle;
        }
    }

    public class ImageDisplayParameterChangedDataEvent : DataEvent
    {
        public ImageId ImageId { get; }
        public DisplayParameter DisplayParameter { get; }

        public ImageDisplayParameterChangedDataEvent(ImageId imageId, DisplayParameter displayParameter)
        {
            ImageId = imageId;
            DisplayParameter = displayParameter;
        }
    }

    public class ImageUnpinnedDataEvent : DataEvent
    {
        public ImageId ImageId { get; }

        public ImageUnpinnedDataEvent(ImageId imageId)
        {
            ImageId = imageId;
        }
    }

    public class ImageRepinnedDataEvent : DataEvent
    {
        public ImageId ImageId { get; }

        public ImageRepinnedDataEvent(ImageId imageId)
        {
            ImageId = imageId;
        }
    }

    public class ImageRemovedDataEvent : DataEvent
    {
        public ImageId ImageId { get; }

        public ImageRemovedDataEvent(ImageId imageId)
        {
            ImageId = imageId;
        }
    }
}