using Core;
using FluentResults;
using Shared;

namespace Application.Services
{
    public interface IUnpinImageService
    {
        Task<Result> Unpin(ImageId imageId, CancellationToken cancellationToken = default);
    }

    public class UnpinImageService : IUnpinImageService
    {
        private readonly IPinnedImageRepository _repository;

        public UnpinImageService(IPinnedImageRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result> Unpin(ImageId imageId, CancellationToken cancellationToken = default)
        {
            PinnedImage? pinnedImage = await _repository.FindAsync(
                specification: new PinnedImageByImageIdSpecification(imageId),
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
