using Data.Common.Contracts;
using Data.Projections;
using LiteDB;
using Shared;

namespace Infrastructure.Data
{
    public class LiteDbAllPinnedImageListItemsQuery : IAsyncQuery<IEnumerable<PinnedImageListItem>>
    {
        private readonly string _connectionString;

        public LiteDbAllPinnedImageListItemsQuery(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task<IEnumerable<PinnedImageListItem>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var db = new LiteDatabase(connectionString: _connectionString);

            ILiteCollection<FlattenedPinnedImageDataHolder> pinnedImages = db.GetCollection<FlattenedPinnedImageDataHolder>();

            pinnedImages.EnsureIndex(x => x.Id, unique: true);

            IEnumerable<PinnedImageListItem> items = (from item in pinnedImages.Find(query: Query.All(field: nameof(PinnedImageListItem.CreationTimestamp), order: Query.Descending))
                                                      select new PinnedImageListItem(
                                                          ImageId: new ImageId(Value: item.Id),
                                                          Directory: new ImageDirectory(Path: item.ImageDirectory),
                                                          Caption: new Caption(
                                                              Text: item.CaptionText,
                                                              IsVisible: item.CaptionVisible),
                                                          IsShown: item.IsShown,
                                                          CreationTimestamp: item.CreationTimestamp)).ToList();

            return Task.FromResult(items);
        }
    }
}