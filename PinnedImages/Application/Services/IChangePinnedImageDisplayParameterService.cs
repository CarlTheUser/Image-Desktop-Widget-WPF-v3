using Core;
using Data.Common.Contracts.SpecificationRepositories;
using FluentResults;
using Shared;
using S = Data.Common.Contracts.SpecificationRepositories;

namespace Application.Services
{
    public interface IChangePinnedImageDisplayParameterService
    {
        Task<Result> Apply(ImageId imageId, DisplayParameter displayParameter, CancellationToken cancellationToken = default);
    }

    public class ChangePinnedImageDisplayParameterService : IChangePinnedImageDisplayParameterService
    {
        private readonly S.IAsyncRepository<ImageId, Core.PinnedImage> _repository;

        public ChangePinnedImageDisplayParameterService(IAsyncRepository<ImageId, PinnedImage> repository)
        {
            _repository = repository;
        }

        public async Task<Result> Apply(ImageId imageId, DisplayParameter displayParameter, CancellationToken cancellationToken = default)
        {
            PinnedImage? pinnedImage = await _repository.FindAsync(specification: new KeySpecification<PinnedImage, ImageId>(key: imageId),
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
