using Application.Services;
using Core;
using FluentAssertions;
using NSubstitute;
using Shared;
using Xunit;

namespace Application.Tests
{
    public class ChangePinnedImageDisplayParameterServiceTest
    {
        private readonly IChangePinnedImageDisplayParameterService _changePinnedImageDisplayParameterService;

        private readonly IPinnedImageRepository _repository
            = Substitute.For<IPinnedImageRepository>();

        public ChangePinnedImageDisplayParameterServiceTest()
        {
            _changePinnedImageDisplayParameterService = new ChangePinnedImageDisplayParameterService(
                repository: _repository);
        }

        [Fact]
        public async Task Apply_Should_Return_OKResult_When_PinnedImage_Is_Valid()
        {
            //Arrange
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
                isPinned: false,
                creationTimestamp: DateTime.Now);

            var displayParameter = new DisplayParameter(
               Dimension: new Dimension(Width: 400, Height: 400),
               Location: new Location(X: 200, Y: 200));

            _repository.FindAsync(
                specification: Arg.Is<PinnedImageByImageIdSpecification>(x => x.ImageId == imageId),
                cancellationToken: Arg.Any<CancellationToken>())
                .Returns(pinnedImage);

            _repository.SaveAsync(pinnedImage, Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            //Act
            var result = await _changePinnedImageDisplayParameterService.Apply(
                imageId: imageId,
                displayParameter: displayParameter);

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

            var displayParameter = new DisplayParameter(
               Dimension: new Dimension(Width: 400, Height: 400),
               Location: new Location(X: 200, Y: 200));

            var expectedErrorMessage = $"Pinned Image \"{searchId}\" not found.";

            _repository.FindAsync(
                specification: Arg.Is<PinnedImageByImageIdSpecification>(x => x.ImageId == pinnedImage.Id),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(pinnedImage);

            _repository.SaveAsync(pinnedImage, Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            //Act
            var result = await _changePinnedImageDisplayParameterService.Apply(
               imageId: searchId,
               displayParameter: displayParameter);

            //Assert
            result.IsFailed.Should().BeTrue();
            expectedErrorMessage.Should().Be(result.Errors.FirstOrDefault()?.Message);
        }
    }
}
