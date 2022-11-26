using Shared;

namespace Data.Projections
{
    public record PinnedImage(
       ImageId Id,
       ImageDirectory Directory,
       Dimension Dimension,
       Location Location,
       ImageColor Color,
       FrameThickness FrameThickness,
       Rotation Rotaion,
       Corner Corner,
       Caption Caption,
       Shadow Shadow,
       bool IsShown,
       DateTime CreationTimestamp);
}