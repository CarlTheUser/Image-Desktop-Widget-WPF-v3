using Core;
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
        private readonly IPinnedImageRepository _repository;
        private readonly string _pinnedImagesDirectory;
        private readonly IFileSystemDeleteHelper _fileSystemDeleteHelper;

        public DeletePinnedImageService(
            IPinnedImageRepository repository,
            string pinnedImagesDirectory,
            IFileSystemDeleteHelper fileSystemDeleteHelper)
        {
            _repository = repository;
            _pinnedImagesDirectory = pinnedImagesDirectory;
            _fileSystemDeleteHelper = fileSystemDeleteHelper;
        }

        public async Task<Result> DeletePinnedImage(ImageId imageId, CancellationToken cancellationToken = default)
        {
            PinnedImage? pinnedImage = await _repository.FindAsync(
                specification: new PinnedImageByImageIdSpecification(imageId),
                cancellationToken: cancellationToken);

            if (pinnedImage == null)
            {
                return Result.Fail(errorMessage: $"Pinned Image \"{imageId}\" not found.");
            }

            pinnedImage.Remove();

            await _repository.SaveAsync(pinnedImage, cancellationToken);

            _fileSystemDeleteHelper.DeleteDirectory(path: Path.Combine(_pinnedImagesDirectory, pinnedImage.Directory));

            return Result.Ok();
        }

        public interface IFileSystemDeleteHelper
        {
            void DeleteDirectory(string path);
        }

        public class FileSystemDeleteHelper : IFileSystemDeleteHelper
        {
            //https://stackoverflow.com/questions/925192/recursive-delete-of-files-and-directories-in-c-sharp
            public void DeleteDirectory(string path)
            {
                var dir = new DirectoryInfo(path);
                if (!dir.Exists) return;

                foreach (var subDir in dir.EnumerateDirectories())
                {
                    DeleteDirectory(subDir.FullName);
                }
                dir.Delete(true);
            }
        }
    }
}
