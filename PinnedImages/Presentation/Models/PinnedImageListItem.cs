using System;

namespace Presentation.Models
{
    public class PinnedImageListItem : BindObservable
    {
        private bool _isShown;

        public Shared.ImageId Id { get; }
        public Shared.ImageDirectory Directory { get; }
        public string CaptionText { get; }
        public bool IsShown
        {
            get => _isShown;
            set
            {
                _isShown = value;
                OnPropertyChanged(nameof(IsShown));
            }
        }
        public DateTime CreationTimestamp { get; }

        public PinnedImageListItem(
            Shared.ImageId id,
            Shared.ImageDirectory directory,
            string caption,
            bool isShown,
            DateTime creationTimestamp)
        {
            Id = id;
            Directory = directory;
            CaptionText = caption;
            _isShown = isShown;
            CreationTimestamp = creationTimestamp;
        }
    }
}
