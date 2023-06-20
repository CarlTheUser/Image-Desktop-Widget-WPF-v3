using Data.Common.Contracts;
using Data.Projections;
using LiteDB;
using Shared;

using P = Data.Projections;

namespace Infrastructure.Data
{
    public class LiteDbPinnedImagesByImageIdQuery : IAsyncQuery<P.PinnedImage?, Shared.ImageId>
    {
        private readonly string _connectionString;

        public LiteDbPinnedImagesByImageIdQuery(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task<PinnedImage?> ExecuteAsync(ImageId parameter, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var db = new LiteDatabase(connectionString: _connectionString);

            ILiteCollection<FlattenedPinnedImageDataHolder> pinnedImages = db.GetCollection<FlattenedPinnedImageDataHolder>();

            pinnedImages.EnsureIndex(x => x.Id, unique: true);

            FlattenedPinnedImageDataHolder? pinnedImage = pinnedImages.Find(x => x.Id == parameter.Value).FirstOrDefault();

            return Task.FromResult(pinnedImage != null ? new P.PinnedImage(
                Id: new ImageId(Value: pinnedImage.Id),
                Directory: new ImageDirectory(Path: pinnedImage.ImageDirectory),
                Dimension: new Dimension(
                    Width: pinnedImage.DimensionWidth,
                    Height: pinnedImage.DimensionHeight),
                Location: new Location(
                    X: pinnedImage.LocationX,
                    Y: pinnedImage.LocationY),
                Color: new ImageColor(HexValue: pinnedImage.ImageColorHexValue),
                FrameThickness: new FrameThickness(Value: pinnedImage.FrameThicknessValue),
                Rotaion: new Rotation(Angle: pinnedImage.RotationAngle),
                Corner: new Corner(Radius: pinnedImage.CornerRadius),
                Caption: new Caption(
                    Text: pinnedImage.CaptionText,
                    IsVisible: pinnedImage.CaptionVisible),
                Shadow: new Shadow(
                    Opacity: pinnedImage.ShadowOpacity,
                    Depth: pinnedImage.ShadowDepth,
                    Direction: pinnedImage.ShadowDirection,
                    BlurRadius: pinnedImage.ShadowBlurRadius,
                    IsVisible: pinnedImage.ShadowVisible),
                IsShown: pinnedImage.IsShown,
                CreationTimestamp: pinnedImage.CreationTimestamp) : null);
        }
    }
}