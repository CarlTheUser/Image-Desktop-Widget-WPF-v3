using Core;
using Data.Common.Contracts.SpecificationRepositories;
using FluentResults;
using Shared;

namespace Application.Services
{
    public interface IDeletePinnedImageService
    {
        Task<Result> DeletePinnedImage(ImageId imageId, CancellationToken cancellationToken = default);

    }

    public class DeletePinnedImageService : IDeletePinnedImageService
    {
        private readonly IAsyncRepository<ImageId, Core.PinnedImage> _repository;
        private readonly DirectoryInfo _pinnedImagesDirectory;

        public DeletePinnedImageService(IAsyncRepository<ImageId, PinnedImage> repository, DirectoryInfo pinnedImagesDirectory)
        {
            _repository = repository;
            _pinnedImagesDirectory = pinnedImagesDirectory;
        }

        public async Task<Result> DeletePinnedImage(ImageId imageId, CancellationToken cancellationToken = default)
        {
            PinnedImage? pinnedImage = await _repository.FindAsync(
                specification: new KeySpecification<PinnedImage, ImageId>(key: imageId),
                cancellationToken: cancellationToken);

            if (pinnedImage == null)
            {
                return Result.Fail(errorMessage: $"Pinned Image \"{imageId}\" not found.");
            }

            pinnedImage.Remove();

            await _repository.SaveAsync(pinnedImage, cancellationToken);

            RecursiveDelete(baseDir: new DirectoryInfo(path: Path.Combine(_pinnedImagesDirectory.FullName, pinnedImage.Directory)));

            return Result.Ok();
        }

        //https://stackoverflow.com/questions/925192/recursive-delete-of-files-and-directories-in-c-sharp
        private static void RecursiveDelete(DirectoryInfo baseDir)
        {
            if (!baseDir.Exists)
                return;

            foreach (var dir in baseDir.EnumerateDirectories())
            {
                RecursiveDelete(dir);
            }
            baseDir.Delete(true);
        }
    }
}
