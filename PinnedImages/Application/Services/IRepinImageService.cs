using Core;
using Data.Common.Contracts.SpecificationRepositories;
using Shared;
using S = Data.Common.Contracts.SpecificationRepositories;

namespace Application.Services
{
    public interface IRepinImageService
    {
        Task Repin(ImageId imageId, CancellationToken cancellationToken = default);
    }

    public class RepinImageService : IRepinImageService
    {
        private readonly S.IAsyncRepository<ImageId, Core.PinnedImage> _repository;

        public RepinImageService(IAsyncRepository<ImageId, PinnedImage> repository)
        {
            _repository = repository;
        }

        public async Task Repin(ImageId imageId, CancellationToken cancellationToken = default)
        {
            PinnedImage? pinnedImage = await _repository.FindAsync(specification: new KeySpecification<PinnedImage, ImageId>(key: imageId),
                                                          cancellationToken: cancellationToken);

            if (pinnedImage == null)
            {
                throw new ApplicationLogicException($"Pinned Image \"{imageId}\" not found.");
            }

            pinnedImage.Pin();

            await _repository.SaveAsync(pinnedImage, cancellationToken);
        }
    }
}
