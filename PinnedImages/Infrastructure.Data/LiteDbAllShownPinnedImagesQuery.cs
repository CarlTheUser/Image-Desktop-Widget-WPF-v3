using Data.Common.Contracts;
using LiteDB;
using Shared;

using P = Data.Projections;

namespace Infrastructure.Data
{
    public class LiteDbAllShownPinnedImagesQuery : IAsyncQuery<IEnumerable<P.PinnedImage>>
    {
        private readonly string _connectionString;

        public LiteDbAllShownPinnedImagesQuery(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task<IEnumerable<P.PinnedImage>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var db = new LiteDatabase(connectionString: _connectionString);

            ILiteCollection<FlattenedPinnedImageDataHolder> pinnedImages = db.GetCollection<FlattenedPinnedImageDataHolder>();

            pinnedImages.EnsureIndex(x => x.Id, unique: true);

            IEnumerable<FlattenedPinnedImageDataHolder> shown = pinnedImages.Find(x => x.IsShown).ToList();

            return Task.FromResult(from item in shown
                                   select new P.PinnedImage(
                                       Id: new ImageId(Value: item.Id),
                                       Directory: new ImageDirectory(Path: item.ImageDirectory),
                                       Dimension: new Dimension(
                                           Width: item.DimensionWidth,
                                           Height: item.DimensionHeight),
                                       Location: new Location(
                                           X: item.LocationX,
                                           Y: item.LocationY),
                                       Color: new ImageColor(HexValue: item.ImageColorHexValue),
                                       FrameThickness: new FrameThickness(Value: item.FrameThicknessValue),
                                       Rotaion: new Rotation(Angle: item.RotationAngle),
                                       Corner: new Corner(Radius: item.CornerRadius),
                                       Caption: new Caption(
                                           Text: item.CaptionText,
                                           IsVisible: item.CaptionVisible),
                                       Shadow: new Shadow(
                                           Opacity: item.ShadowOpacity,
                                           Depth: item.ShadowDepth,
                                           Direction: item.ShadowDirection,
                                           BlurRadius: item.ShadowBlurRadius,
                                           IsVisible: item.ShadowVisible),
                                       IsShown: item.IsShown,
                                       CreationTimestamp: item.CreationTimestamp));
        }
    }
}