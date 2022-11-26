using Shared;

namespace Data.Projections
{
    public record PinnedImageListItem(
       ImageId ImageId,
       ImageDirectory Directory,
       Caption Caption,
       bool IsShown,
       DateTime CreationTimestamp);
}