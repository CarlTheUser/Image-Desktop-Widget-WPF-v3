using Core;
using Data.Common.Contracts.SpecificationRepositories;
using FluentResults;
using Shared;
using S = Data.Common.Contracts.SpecificationRepositories;

namespace Application.Services
{
    public interface IUnpinImageService
    {
        Task<Result> Unpin(ImageId imageId, CancellationToken cancellationToken = default);
    }

    public class UnpinImageService : IUnpinImageService
    {
        private readonly S.IAsyncRepository<ImageId, Core.PinnedImage> _repository;

        public UnpinImageService(IAsyncRepository<ImageId, PinnedImage> repository)
        {
            _repository = repository;
        }

        public async Task<Result> Unpin(ImageId imageId, CancellationToken cancellationToken = default)
        {
            PinnedImage? pinnedImage = await _repository.FindAsync(specification: new KeySpecification<PinnedImage, ImageId>(key: imageId),
                                                          cancellationToken: cancellationToken);

            if (pinnedImage == null)
            {
                return Result.Fail($"Pinned Image \"{imageId}\" not found.");
            }

            pinnedImage.UnPin();

            await _repository.SaveAsync(pinnedImage, cancellationToken);

            return Result.Ok();
        }
    }
}
