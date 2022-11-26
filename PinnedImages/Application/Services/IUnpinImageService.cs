using Core;
using Data.Common.Contracts.SpecificationRepositories;
using Shared;
using S = Data.Common.Contracts.SpecificationRepositories;

namespace Application.Services
{
    public interface IUnpinImageService
    {
        Task Unpin(ImageId imageId, CancellationToken cancellationToken = default);
    }

    public class UnpinImageService : IUnpinImageService
    {
        private readonly S.IAsyncRepository<ImageId, Core.PinnedImage> _repository;

        public UnpinImageService(IAsyncRepository<ImageId, PinnedImage> repository)
        {
            _repository = repository;
        }

        public async Task Unpin(ImageId imageId, CancellationToken cancellationToken = default)
        {
            PinnedImage? pinnedImage = await _repository.FindAsync(specification: new KeySpecification<PinnedImage, ImageId>(key: imageId),
                                                          cancellationToken: cancellationToken);

            if (pinnedImage == null)
            {
                throw new ApplicationLogicException($"Pinned Image \"{imageId}\" not found.");
            }

            pinnedImage.UnPin();

            await _repository.SaveAsync(pinnedImage, cancellationToken);
        }
    }
}
