// See https://aka.ms/new-console-template for more information
using Core;
using Data.Common.Contracts.SpecificationRepositories;
using Shared;

Console.WriteLine("Hello, World!");

//var stubRepo = new StubPinnedImageRepository();
//var liteDbRepo = new LiteDBPinnedImageRepository(@".\AppDb");

//using var fs = new FileStream(path: @".\dahyun.jpg", mode: FileMode.Open, access: FileAccess.Read);

//IPinImageService _service = new PinImageService(
//    relativeFolderNameLength: 10,
//    repository: liteDbRepo,
//    thumbnailLength: 300,
//    pinnedImagesDirectory: new DirectoryInfo(path: @".\Images"),
//    randomStringGenerator: new AbcRandomStringGenerator());

//var image = await _service.PinImage(
//    request: new IPinImageService.PinImageRequest(
//        ImageStream: fs,
//        Dimension: new Dimension(Width: 350, Height: 225),
//        Location: new Location(X: 300, Y: 225),
//        Color: new ImageColor(HexValue: "#FFFFFF"),
//        FrameThickness: new FrameThickness(Value: 25),
//        Rotation: new Rotation(Angle: 35),
//        Caption: Caption.None,
//        Shadow: Shadow.None),
//    cancellationToken: CancellationToken.None);

//var found = await liteDbRepo.FindAsync(specification: new KeySpecification<Core.PinnedImage, ImageId>(image.Id), CancellationToken.None);

//Console.ReadKey();



class StubPinnedImageRepository : IPinnedImageRepository
{
    public Task<PinnedImage?> FindAsync(PinnedImageByImageIdSpecification specification, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<Core.PinnedImage?>(result: null);
    }

    public Task SaveAsync(Core.PinnedImage item, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

   
}

