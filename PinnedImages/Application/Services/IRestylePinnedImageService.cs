using Core;
using FluentResults;
using Shared;

namespace Application.Services
{
    public interface IRestylePinnedImageService
    {
        Task<Result> Apply(ImageId imageId, VisualStyle style, CancellationToken cancellationToken = default);
    }

    public class RestylePinnedImageService : IRestylePinnedImageService
    {
        private readonly IPinnedImageRepository _repository;

        public RestylePinnedImageService(IPinnedImageRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result> Apply(ImageId imageId, VisualStyle style, CancellationToken cancellationToken = default)
        {
            PinnedImage? pinnedImage = await _repository.FindAsync(
                specification: new PinnedImageByImageIdSpecification(imageId),
                cancellationToken: cancellationToken);

            if (pinnedImage == null)
            {
                return Result.Fail(errorMessage: $"Pinned Image \"{imageId}\" not found.");
            }

            pinnedImage.ApplyVisualStyle(style);

            await _repository.SaveAsync(pinnedImage, cancellationToken);

            return Result.Ok();
        }
    }
}
