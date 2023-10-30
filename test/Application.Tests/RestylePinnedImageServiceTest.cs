using Application.Services;
using Core;
using Data.Common.Contracts.SpecificationRepositories;
using FluentAssertions;
using NSubstitute;
using Shared;
using Xunit;
using S = Data.Common.Contracts.SpecificationRepositories;

namespace Application.Tests
{
    public class RestylePinnedImageServiceTest
    {
        private readonly IRestylePinnedImageService _restylePinnedImageService;
        private readonly IPinnedImageRepository _repository 
            = Substitute.For<IPinnedImageRepository>();

        public RestylePinnedImageServiceTest()
        {
            _restylePinnedImageService = new RestylePinnedImageService(_repository);
        }

        [Fact]
        public async Task Apply_Should_Return_OKResult_When_PinnedImage_Exists()
        {
            //Arange
            var imageId = ImageId.New();

            var pinnedImage = PinnedImage.Existing(
                id: imageId,
                directory: new ImageDirectory(""),
                dimension: new Dimension(Width: 0, Height: 0),
                location: new Location(X: 0, Y: 0),
                color: new ImageColor(HexValue: ""),
                frameThickness: new FrameThickness(Value: 0),
                rotation: Rotation.Zero,
                corner: Corner.None,
                caption: Caption.None,
                shadow: Shadow.None,
                isPinned: true,
                creationTimestamp: DateTime.Now);

            var visualStyle = new VisualStyle(
                Color: new ImageColor(""),
                FrameThickness: new FrameThickness(Value: 0),
                Rotation: new Rotation(0),
                Corner: new Corner(Radius: 0),
                Caption: new Caption("", IsVisible: true),
                Shadow: new Shadow(
                    Opacity: 0,
                    Depth: 0,
                    Direction: 0,
                    BlurRadius: 0,
                    IsVisible: true));

            _repository.FindAsync(
                specification: Arg.Is<PinnedImageByImageIdSpecification>(x => x.ImageId == imageId),
                cancellationToken: Arg.Any<CancellationToken>())
                .Returns(pinnedImage);

            _repository.SaveAsync(pinnedImage, Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            //Act
            var result = await _restylePinnedImageService.Apply(
                imageId: imageId,
                style: visualStyle);

            //Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Apply_Should_Return_FailResult_When_PinnedImage_Not_Exists()
        {
            //Arange
            var searchId = ImageId.New();

            var pinnedImage = PinnedImage.Existing(
                id: ImageId.New(),
                directory: new ImageDirectory(""),
                dimension: new Dimension(Width: 0, Height: 0),
                location: new Location(X: 0, Y: 0),
                color: new ImageColor(HexValue: ""),
                frameThickness: new FrameThickness(Value: 0),
                rotation: Rotation.Zero,
                corner: Corner.None,
                caption: Caption.None,
                shadow: Shadow.None,
                isPinned: true,
                creationTimestamp: DateTime.Now);

            var visualStyle = new VisualStyle(
                Color: new ImageColor(""),
                FrameThickness: new FrameThickness(Value: 0),
                Rotation: new Rotation(15),
                Corner: new Corner(Radius: 12),
                Caption: new Caption("", IsVisible: true),
                Shadow: new Shadow(
                    Opacity: 0,
                    Depth: 0,
                    Direction: 0,
                    BlurRadius: 0,
                    IsVisible: true));

            var expectedErrorMessage = $"Pinned Image \"{searchId}\" not found.";

            _repository.FindAsync(
                specification: Arg.Is<PinnedImageByImageIdSpecification>(x => x.ImageId == pinnedImage.Id),
                cancellationToken: Arg.Any<CancellationToken>())
                .Returns(pinnedImage);

            _repository.SaveAsync(pinnedImage, Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            //Act
            var result = await _restylePinnedImageService.Apply(
                imageId: searchId,
                style: visualStyle);

            //Assert
            result.IsFailed.Should().BeTrue();
            expectedErrorMessage.Should().Be(result.Errors.FirstOrDefault()?.Message);
        }
    }
}