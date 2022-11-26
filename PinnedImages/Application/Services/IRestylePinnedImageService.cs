using Core;
using Data.Common.Contracts.SpecificationRepositories;
using Shared;
using S = Data.Common.Contracts.SpecificationRepositories;

namespace Application.Services
{
    public interface IRestylePinnedImageService
    {
        Task Apply(ImageId imageId, VisualStyle style, CancellationToken cancellationToken = default);
    }

    public class RestylePinnedImageService : IRestylePinnedImageService
    {
        private readonly S.IAsyncRepository<ImageId, Core.PinnedImage> _repository;

        public RestylePinnedImageService(IAsyncRepository<ImageId, PinnedImage> repository)
        {
            _repository = repository;
        }

        public async Task Apply(ImageId imageId, VisualStyle style, CancellationToken cancellationToken = default)
        {
            PinnedImage? pinnedImage = await _repository.FindAsync(specification: new KeySpecification<PinnedImage, ImageId>(key: imageId),
                                                          cancellationToken: cancellationToken);

            if (pinnedImage == null)
            {
                throw new ApplicationLogicException($"Pinned Image \"{imageId}\" not found.");
            }

            pinnedImage.ApplyVisualStyle(style);

            await _repository.SaveAsync(pinnedImage, cancellationToken);
        }
    }
}
