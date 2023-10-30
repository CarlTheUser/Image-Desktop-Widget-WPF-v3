using Core;
using FluentResults;
using Shared;

namespace Application.Services
{
    public interface IChangePinnedImageDisplayParameterService
    {
        Task<Result> Apply(ImageId imageId, DisplayParameter displayParameter, CancellationToken cancellationToken = default);
    }

    public class ChangePinnedImageDisplayParameterService : IChangePinnedImageDisplayParameterService
    {
        private readonly IPinnedImageRepository _repository;

        public ChangePinnedImageDisplayParameterService(IPinnedImageRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result> Apply(ImageId imageId, DisplayParameter displayParameter, CancellationToken cancellationToken = default)
        {
            PinnedImage? pinnedImage = await _repository.FindAsync(
                specification: new PinnedImageByImageIdSpecification(imageId),
                cancellationToken: cancellationToken);

            if (pinnedImage == null)
            {
                return Result.Fail(errorMessage: $"Pinned Image \"{imageId}\" not found.");
            }

            pinnedImage.ApplyParameters(displayParameter);

            await _repository.SaveAsync(pinnedImage, cancellationToken);

            return Result.Ok();
        }
    }
}
