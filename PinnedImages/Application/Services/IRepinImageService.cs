using Core;
using FluentResults;
using Shared;

namespace Application.Services
{
    public interface IRepinImageService
    {
        Task<Result> Repin(ImageId imageId, CancellationToken cancellationToken = default);
    }

    public class RepinImageService : IRepinImageService
    {
        private readonly IPinnedImageRepository _repository;

        public RepinImageService(IPinnedImageRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result> Repin(ImageId imageId, CancellationToken cancellationToken = default)
        {
            PinnedImage? pinnedImage = await _repository.FindAsync(
                specification: new PinnedImageByImageIdSpecification(ImageId: imageId),
                cancellationToken: cancellationToken);

            if (pinnedImage == null)
            {
                return Result.Fail(errorMessage: $"Pinned Image \"{imageId}\" not found.");
            }

            pinnedImage.Pin();

            await _repository.SaveAsync(pinnedImage, cancellationToken);

            return Result.Ok();
        }
    }
}
