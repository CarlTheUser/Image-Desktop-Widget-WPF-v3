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
    public class RepinImageServiceTest
    {
        private readonly IRepinImageService _repinImageService;
        private readonly IPinnedImageRepository _repository
            = Substitute.For<IPinnedImageRepository>();

        public RepinImageServiceTest()
        {
            _repinImageService = new RepinImageService(_repository);
        }

        [Fact]
        public async Task Repin_Should_Return_OKResult_When_PinnedImage_Exists_And_State_Is_Unpinned()
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
                isPinned: false,
                creationTimestamp: DateTime.Now);

            _repository.FindAsync(
                specification: Arg.Is<PinnedImageByImageIdSpecification>(x => x.ImageId == imageId),
                cancellationToken: Arg.Any<CancellationToken>())
                .Returns(pinnedImage);

            _repository.SaveAsync(pinnedImage, Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            //Act
            var result = await _repinImageService.Repin(imageId: imageId);

            //Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Repin_Should_Return_FailResult_When_PinnedImage_Not_Exists()
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

            var expectedErrorMessage = $"Pinned Image \"{searchId}\" not found.";

            _repository.FindAsync(
                specification: Arg.Is<PinnedImageByImageIdSpecification>(x => x.ImageId == pinnedImage.Id),
                cancellationToken: Arg.Any<CancellationToken>())
                .Returns(pinnedImage);

            _repository.SaveAsync(pinnedImage, Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            //Act
            var result = await _repinImageService.Repin(imageId: searchId);

            //Assert
            result.IsFailed.Should().BeTrue();
            expectedErrorMessage.Should().Be(result.Errors.FirstOrDefault()?.Message);
        }

        [Fact]
        public async Task Repin_Should_Throw_When_PinnedImage_Exists_And_State_Is_Already_Pinned()
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

            var expectedErrorMessage = "Image is already pinned.";

            _repository.FindAsync(
                specification: Arg.Is<PinnedImageByImageIdSpecification>(x => x.ImageId == imageId),
                cancellationToken: Arg.Any<CancellationToken>())
                .Returns(pinnedImage);

            _repository.SaveAsync(pinnedImage, Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            //Act
            Func<Task> act = async () => await _repinImageService.Repin(imageId: imageId);

            //Assert
            await act.Should()
                .ThrowAsync< DomainLogicException >()
                .WithMessage(expectedErrorMessage);
        }
    }
}
