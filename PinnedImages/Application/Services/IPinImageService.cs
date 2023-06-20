using FluentResults;
using Misc.Utilities;
using Shared;
using S = Data.Common.Contracts.SpecificationRepositories;

namespace Application.Services
{
    public interface IPinImageService
    {
        public record PinImageRequest(
            Stream ImageStream,
            Dimension Dimension,
            Location Location,
            ImageColor Color,
            FrameThickness FrameThickness,
            Rotation Rotation,
            Corner Corner,
            Caption Caption,
            Shadow Shadow);

        Task<Result<Data.Projections.PinnedImage>> PinImage(PinImageRequest request, CancellationToken cancellationToken = default);
    }

    public class PinImageService : IPinImageService
    {
        private readonly int _relativeFolderNameLength;
        private readonly S.IAsyncRepository<ImageId, Core.PinnedImage> _repository;
        private readonly int _thumbnailLength;
        private readonly DirectoryInfo _pinnedImagesDirectory;
        private readonly IRandomStringGenerator _randomStringGenerator;

        public PinImageService(int relativeFolderNameLength,
            S.IAsyncRepository<ImageId, Core.PinnedImage> repository,
            int thumbnailLength,
            DirectoryInfo pinnedImagesDirectory,
            IRandomStringGenerator randomStringGenerator)
        {
            if (relativeFolderNameLength < 3) throw new ArgumentException(message: "Relative folder name length should not be less than 3.", paramName: nameof(relativeFolderNameLength));
            _relativeFolderNameLength = relativeFolderNameLength;
            _repository = repository;
            _thumbnailLength = thumbnailLength;
            _pinnedImagesDirectory = pinnedImagesDirectory;
            _randomStringGenerator = randomStringGenerator;
        }

        public async Task<Result<Data.Projections.PinnedImage>> PinImage(IPinImageService.PinImageRequest request, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!_pinnedImagesDirectory.Exists)
            {
                _pinnedImagesDirectory.Create();
            }

            request.Deconstruct(
                out Stream originalImageStream,
                out Dimension dimension,
                out Location location,
                out ImageColor color,
                out FrameThickness frameThickness,
                out Rotation rotation,
                out Corner corner,
                out Caption caption,
                out Shadow shadow);

            string imageFolderName = _randomStringGenerator.Generate(_relativeFolderNameLength);

            string imageFolderPath = Path.Combine(_pinnedImagesDirectory.FullName, imageFolderName);

            Directory.CreateDirectory(imageFolderPath);

            using var thumbnailStream = new MemoryStream();

            originalImageStream.Position = 0;

            await originalImageStream.CopyToAsync(destination: thumbnailStream, cancellationToken: cancellationToken);

            thumbnailStream.Position = 0;

            using var magickImage = new ImageMagick.MagickImage(stream: thumbnailStream);

            if (magickImage.BaseHeight > magickImage.BaseWidth)
            {
                magickImage.Resize(width: 0, height: _thumbnailLength);
            }
            else
            {
                magickImage.Resize(width: _thumbnailLength, height: 0);
            }

            await magickImage.WriteAsync(fileName: Path.Combine(imageFolderPath, "thumbnail"), cancellationToken: cancellationToken);

            using var originalFileSteam = new FileStream(
                path: Path.Combine(imageFolderPath, "original"),
                mode: FileMode.Create,
                access: FileAccess.ReadWrite);

            originalImageStream.Position = 0;

            await originalImageStream.CopyToAsync(destination: originalFileSteam, cancellationToken: cancellationToken);

            originalFileSteam.Position = 0;

            Core.PinnedImage pinnedImage = Core.PinnedImage.New(
                imageDirectory: new ImageDirectory(Path: imageFolderName),
                dimension: dimension,
                location: location,
                color: color,
                frameThickness: frameThickness,
                rotation: rotation,
                corner: corner,
                caption: caption,
                shadow: shadow);

            await _repository.SaveAsync(
                item: pinnedImage,
                cancellationToken: cancellationToken);

            return Result.Ok(value: new Data.Projections.PinnedImage(
                Id: pinnedImage.Id,
                Directory: pinnedImage.Directory,
                Dimension: pinnedImage.Dimension,
                Location: pinnedImage.Location,
                Color: pinnedImage.Color,
                FrameThickness: pinnedImage.FrameThickness,
                Rotaion: pinnedImage.Rotation,
                Corner: pinnedImage.Corner,
                Caption: pinnedImage.Caption,
                Shadow: pinnedImage.Shadow,
                IsShown: pinnedImage.IsPinned,
                CreationTimestamp: pinnedImage.CreationTimestamp));
        }
    }
}
