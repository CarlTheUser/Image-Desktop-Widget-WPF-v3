using Core.Events;
using Data.Common.Contracts;
using LiteDB;
using Shared;
using S = Data.Common.Contracts.SpecificationRepositories;

namespace Infrastructure.Data
{
    public class LiteDBPinnedImageRepository : S.IAsyncRepository<ImageId, Core.PinnedImage>
    {
        private readonly string _connectionString;

        public LiteDBPinnedImageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task<Core.PinnedImage?> FindAsync(S.KeySpecification<Core.PinnedImage, ImageId> specification, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var db = new LiteDatabase(connectionString: _connectionString);

            ILiteCollection<FlattenedPinnedImageDataHolder> pinnedImages = db.GetCollection<FlattenedPinnedImageDataHolder>();

            pinnedImages.EnsureIndex(x => x.Id, unique: true);

            FlattenedPinnedImageDataHolder? pinnedImage = pinnedImages.Find(x => x.Id == specification.Key.Value).FirstOrDefault();

            return Task.FromResult(pinnedImage != null ? Core.PinnedImage.Existing(
                id: new ImageId(Value: pinnedImage.Id),
                directory: new ImageDirectory(Path: pinnedImage.ImageDirectory),
                dimension: new Dimension(
                    Width: pinnedImage.DimensionWidth,
                    Height: pinnedImage.DimensionHeight),
                location: new Location(
                    X: pinnedImage.LocationX,
                    Y: pinnedImage.LocationY),
                color: new ImageColor(HexValue: pinnedImage.ImageColorHexValue),
                frameThickness: new FrameThickness(Value: pinnedImage.FrameThicknessValue),
                rotation: new Rotation(Angle: pinnedImage.RotationAngle),
                corner: new Corner(Radius: pinnedImage.CornerRadius),
                caption: new Caption(
                    Text: pinnedImage.CaptionText,
                    IsVisible: pinnedImage.CaptionVisible),
                shadow: new Shadow(
                    Opacity: pinnedImage.ShadowOpacity,
                    Depth: pinnedImage.ShadowDepth,
                    Direction: pinnedImage.ShadowDirection,
                    BlurRadius: pinnedImage.ShadowBlurRadius,
                    IsVisible: pinnedImage.ShadowVisible),
                isPinned: pinnedImage.IsShown,
                creationTimestamp: pinnedImage.CreationTimestamp) : null);
        }

        public Task SaveAsync(Core.PinnedImage item, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var db = new LiteDatabase(connectionString: _connectionString);

            ILiteCollection<FlattenedPinnedImageDataHolder> pinnedImages = db.GetCollection<FlattenedPinnedImageDataHolder>();

            pinnedImages.EnsureIndex(x => x.Id, unique: true);

            IEnumerable<DataEvent> events = item.ReleaseEvents();

            FlattenedPinnedImageDataHolder data;

            foreach (DataEvent @event in events)
            {
                switch (@event)
                {
                    case NewImagePinnedDataEvent nipde:

                        data = new FlattenedPinnedImageDataHolder()
                        {
                            Id = nipde.ImageId,
                            ImageDirectory = nipde.Directory,
                            DimensionWidth = nipde.Dimension.Width,
                            DimensionHeight = nipde.Dimension.Height,
                            LocationX = nipde.Location.X,
                            LocationY = nipde.Location.Y,
                            ImageColorHexValue = nipde.Color,
                            FrameThicknessValue = nipde.FrameThickness,
                            RotationAngle = nipde.Rotation,
                            CornerRadius = nipde.Corner,
                            CaptionText = nipde.Caption.Text,
                            CaptionVisible = nipde.Caption.IsVisible,
                            ShadowBlurRadius = nipde.Shadow.BlurRadius,
                            ShadowDepth = nipde.Shadow.Depth,
                            ShadowDirection = nipde.Shadow.Direction,
                            ShadowOpacity = nipde.Shadow.Opacity,
                            ShadowVisible = nipde.Shadow.IsVisible,
                            IsShown = true,
                            CreationTimestamp = nipde.CreationTimestamp
                        };

                        pinnedImages.Insert(data);

                        break;

                    case ImageRestyledDataEvent irde:

                        data = pinnedImages.FindOne(x => x.Id == irde.ImageId.Value);

                        VisualStyle visualStyle = irde.VisualStyle;

                        data.ImageColorHexValue = visualStyle.Color.HexValue;
                        data.FrameThicknessValue = visualStyle.FrameThickness;
                        data.RotationAngle = visualStyle.Rotaion;
                        data.CornerRadius = visualStyle.Corner.Radius;
                        data.CaptionText = visualStyle.Caption.Text;
                        data.CaptionVisible = visualStyle.Caption.IsVisible;
                        data.ShadowBlurRadius = visualStyle.Shadow.BlurRadius;
                        data.ShadowDepth = visualStyle.Shadow.Depth;
                        data.ShadowDirection = visualStyle.Shadow.Direction;
                        data.ShadowOpacity = visualStyle.Shadow.Opacity;
                        data.ShadowVisible = visualStyle.Shadow.IsVisible;

                        pinnedImages.Update(data);

                        break;

                    case ImageDisplayParameterChangedDataEvent idpcde:

                        data = pinnedImages.FindOne(x => x.Id == idpcde.ImageId.Value);

                        DisplayParameter parameter = idpcde.DisplayParameter;

                        data.DimensionWidth = parameter.Dimension.Width;
                        data.DimensionHeight = parameter.Dimension.Height;
                        data.LocationX = parameter.Location.X;
                        data.LocationY = parameter.Location.Y;

                        pinnedImages.Update(data);

                        break;

                    case ImageUnpinnedDataEvent iude:

                        data = pinnedImages.FindOne(x => x.Id == iude.ImageId.Value);

                        data.IsShown = false;

                        pinnedImages.Update(data);

                        break;

                    case ImageRepinnedDataEvent irde:

                        data = pinnedImages.FindOne(x => x.Id == irde.ImageId.Value);

                        data.IsShown = true;

                        pinnedImages.Update(data);

                        break;

                    case ImageRemovedDataEvent irde:

                        pinnedImages.DeleteMany(x => x.Id == irde.ImageId.Value);

                        break;
                }
            }

            return Task.CompletedTask;
        }
    }
}