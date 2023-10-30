using Core;
using Core.Events;
using FluentAssertions;
using Shared;
using Xunit;

namespace Core.Tests
{
    public class PinnedImageTest
    {
        [Fact]
        public void New_Should_Set_Properties_Correctly()
        {
            //Arange
            var imageDirectory = new ImageDirectory("path");
            var dimension = new Dimension(Width: 300, Height: 300);
            var location = new Location(X: 100, Y: 100);
            var imageColor = new ImageColor(HexValue: "#FFFFFF");
            var frameThickness = new FrameThickness(Value: 15);
            var rotation = Rotation.Zero;
            var corner = Corner.None;
            var caption = Caption.None;
            var shadow = Shadow.None;

            //Act
            var pinnedImage = PinnedImage.New(
                imageDirectory: imageDirectory,
                dimension: dimension,
                location: location,
                color: imageColor,
                frameThickness: frameThickness,
                rotation: rotation,
                corner: corner,
                caption: caption,
                shadow: shadow);

            //Assert
            pinnedImage.Directory.Should().Be(imageDirectory);
            pinnedImage.Dimension.Should().Be(dimension);
            pinnedImage.Location.Should().Be(location);
            pinnedImage.Color.Should().Be(imageColor);
            pinnedImage.FrameThickness.Should().Be(frameThickness);
            pinnedImage.Rotation.Should().Be(rotation);
            pinnedImage.Corner.Should().Be(corner);
            pinnedImage.Caption.Should().Be(caption);
            pinnedImage.Shadow.Should().Be(shadow);
        }

        [Fact]
        public void New_Should_Contain_NewImagePinnedDataEvent_In_Events()
        {
            //Arange
            var imageDirectory = new ImageDirectory("");
            var dimension = new Dimension(Width: 0, Height: 0);
            var location = new Location(X: 0, Y: 0);
            var imageColor = new ImageColor(HexValue: "");
            var frameThickness = new FrameThickness(Value: 0);
            var rotation = Rotation.Zero;
            var corner = Corner.None;
            var caption = Caption.None;
            var shadow = Shadow.None;

            //Act
            var pinnedImage = PinnedImage.New(
                imageDirectory: imageDirectory,
                dimension: dimension,
                location: location,
                color: imageColor,
                frameThickness: frameThickness,
                rotation: rotation,
                corner: corner,
                caption: caption,
                shadow: shadow);

            //Assert
            pinnedImage.Id.Should().Be(pinnedImage.ReleaseEvents().OfType<NewImagePinnedDataEvent>().SingleOrDefault()?.ImageId);
        }

        [Fact]
        public void New_PinnedImage_Should_Be_Pinned_By_Default()
        {
            //Arange
            var imageDirectory = new ImageDirectory("0");
            var dimension = new Dimension(Width: 0, Height: 0);
            var location = new Location(X: 0, Y: 0);
            var imageColor = new ImageColor(HexValue: "");
            var frameThickness = new FrameThickness(Value: 0);
            var rotation = Rotation.Zero;
            var corner = Corner.None;
            var caption = Caption.None;
            var shadow = Shadow.None;

            //Act
            var pinnedImage = PinnedImage.New(
                imageDirectory: imageDirectory,
                dimension: dimension,
                location: location,
                color: imageColor,
                frameThickness: frameThickness,
                rotation: rotation,
                corner: corner,
                caption: caption,
                shadow: shadow);

            //Assert
            pinnedImage.IsPinned.Should().BeTrue();
        }

        [Fact]
        public void Existing_Should_Set_Properties_Correctly()
        {
            //Arange
            var id = ImageId.New();
            var imageDirectory = new ImageDirectory("path");
            var dimension = new Dimension(Width: 300, Height: 300);
            var location = new Location(X: 100, Y: 100);
            var imageColor = new ImageColor(HexValue: "#FFFFFF");
            var frameThickness = new FrameThickness(Value: 15);
            var rotation = Rotation.Zero;
            var corner = Corner.None;
            var caption = Caption.None;
            var shadow = Shadow.None;
            var isPinned = true;
            var creationTimestamp = DateTime.Now;

            //Act
            var pinnedImage = PinnedImage.Existing(
                id: id,
                directory: imageDirectory,
                dimension: dimension,
                location: location,
                color: imageColor,
                frameThickness: frameThickness,
                rotation: rotation,
                corner: corner,
                caption: caption,
                shadow: shadow,
                isPinned: isPinned,
                creationTimestamp: creationTimestamp);

            //Assert
            pinnedImage.Id.Should().Be(id);
            pinnedImage.Directory.Should().Be(imageDirectory);
            pinnedImage.Dimension.Should().Be(dimension);
            pinnedImage.Location.Should().Be(location);
            pinnedImage.Color.Should().Be(imageColor);
            pinnedImage.FrameThickness.Should().Be(frameThickness);
            pinnedImage.Rotation.Should().Be(rotation);
            pinnedImage.Corner.Should().Be(corner);
            pinnedImage.Caption.Should().Be(caption);
            pinnedImage.Shadow.Should().Be(shadow);
            pinnedImage.IsPinned.Should().Be(isPinned);
            pinnedImage.CreationTimestamp.Should().Be(creationTimestamp);
        }

        [Fact]
        public void Existing_Should_Leave_Events_Empty()
        {
            //Arange
            var imageDirectory = new ImageDirectory("");
            var dimension = new Dimension(Width: 0, Height: 0);
            var location = new Location(X: 0, Y: 0);
            var imageColor = new ImageColor(HexValue: "");
            var frameThickness = new FrameThickness(Value: 0);
            var rotation = Rotation.Zero;
            var corner = Corner.None;
            var caption = Caption.None;
            var shadow = Shadow.None;
            var isPinned = true;
            var creationTimestamp = DateTime.Now;

            //Act
            var pinnedImage = PinnedImage.Existing(
                id: ImageId.New(),
                directory: imageDirectory,
                dimension: dimension,
                location: location,
                color: imageColor,
                frameThickness: frameThickness,
                rotation: rotation,
                corner: corner,
                caption: caption,
                shadow: shadow,
                isPinned: isPinned,
                creationTimestamp: creationTimestamp);

            //Assert
            pinnedImage.ReleaseEvents().Should().BeEmpty();
        }

        [Fact]
        public void ApplyVisualStyle_Should_Set_Properties_Correctly()
        {
            //Arange
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
                Color: new ImageColor("#32A852"),
                FrameThickness: new FrameThickness(Value: 25),
                Rotation: new Rotation(15),
                Corner: new Corner(Radius: 12),
                Caption: new Caption("I'm a caption", IsVisible: true),
                Shadow: new Shadow(
                    Opacity: 0.4,
                    Depth: 10,
                    Direction: 270,
                    BlurRadius: 0.43,
                    IsVisible: true));

            //Act
            pinnedImage.ApplyVisualStyle(visualStyle);

            //Assert
            pinnedImage.Color.Should().Be(visualStyle.Color);
            pinnedImage.FrameThickness.Should().Be(visualStyle.FrameThickness);
            pinnedImage.Rotation.Should().Be(visualStyle.Rotation);
            pinnedImage.Corner.Should().Be(visualStyle.Corner);
            pinnedImage.Caption.Should().Be(visualStyle.Caption);
            pinnedImage.Shadow.Should().Be(visualStyle.Shadow);
        }
        
        [Fact]
        public void ApplyVisualStyle_Should_Contain_ImageRestyledDataEvent_In_Events()
        {
            //Arange
            var pinnedImage = PinnedImage.Existing(
                id: ImageId.New(),
                directory: new ImageDirectory(""),
                dimension: new Dimension(Width: 0, Height: 0),
                location: new Location(X: 100, Y: 100),
                color: new ImageColor(HexValue: ""),
                frameThickness: new FrameThickness(Value: 0),
                rotation: Rotation.Zero,
                corner: Corner.None,
                caption: Caption.None,
                shadow: Shadow.None,
                isPinned: true,
                creationTimestamp: DateTime.Now);

            var visualStyle = new VisualStyle(
                Color: new ImageColor(HexValue: ""),
                FrameThickness: new FrameThickness(Value: 0),
                Rotation: Rotation.Zero,
                Corner: Corner.None,
                Caption: Caption.None,
                Shadow: Shadow.None);

            //Act
            pinnedImage.ApplyVisualStyle(visualStyle);

            //Assert
            pinnedImage.ReleaseEvents().Should().ContainItemsAssignableTo<ImageRestyledDataEvent>();
        }

        [Fact]
        public void ApplyParameters_Should_Set_Properties_Correctly()
        {
            //Arange
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

            //Act
            pinnedImage.ApplyParameters(displayParameter);

            //Assert
            pinnedImage.Dimension.Should().Be(displayParameter.Dimension);
            pinnedImage.Location.Should().Be(displayParameter.Location);
        }

        [Fact]
        public void ApplyParameters_Should_Contain_ImageDisplayParameterChangedDataEvent_In_Events()
        {
            //Arange
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
                Dimension: new Dimension(Width: 0, Height: 0),
                Location: new Location(X: 0, Y: 0));

            //Act
            pinnedImage.ApplyParameters(displayParameter);

            //Assert
            pinnedImage.ReleaseEvents().Should().ContainItemsAssignableTo<ImageDisplayParameterChangedDataEvent>();
        }

        [Fact]
        public void Pin_Should_Set_Properties_Correctly()
        {
            //Arange
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
                isPinned: false,
                creationTimestamp: DateTime.Now);

            //Act
            pinnedImage.Pin();

            //Assert
            pinnedImage.IsPinned.Should().BeTrue();
        }

        [Fact]
        public void Pin_Should_Throw_When_Image_Is_Already_Pinned()
        {
            //Arange
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

            //Act
            Action act = () => pinnedImage.Pin();

            //Assert
            act.Should()
                .Throw<DomainLogicException>()
                .WithMessage("Image is already pinned.");
        }

        [Fact]
        public void Pin_Should_Contain_ImageRepinnedDataEvent_In_Events()
        {
            //Arange
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
                isPinned: false,
                creationTimestamp: DateTime.Now);

            //Act
            pinnedImage.Pin();

            //Assert
            pinnedImage.ReleaseEvents().Should().ContainItemsAssignableTo<ImageRepinnedDataEvent>();
        }

        [Fact]
        public void UnPin_Should_Set_Properties_Correctly()
        {
            //Arange
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

            //Act
            pinnedImage.UnPin();

            //Assert
            pinnedImage.IsPinned.Should().BeFalse();
        }

        [Fact]
        public void UnPin_Should_Throw_When_Image_Is_Already_Unpinned()
        {
            //Arange
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
               isPinned: false,
               creationTimestamp: DateTime.Now);

            //Act
            Action act = () => pinnedImage.UnPin();

            //Assert
            act.Should()
                .Throw<DomainLogicException>()
                .WithMessage("Image is already unpinned.");
        }

        [Fact]
        public void UnPin_Should_Contain_ImageUnpinnedDataEvent_In_Events()
        {
            //Arange
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

            //Act
            pinnedImage.UnPin();

            //Assert
            pinnedImage.ReleaseEvents().Should().ContainItemsAssignableTo<ImageUnpinnedDataEvent>();
        }

        [Fact]
        public void Remove_Should_Contain_ImageRemovedDataEvent_In_Events()
        {
            //Arange
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

            //Act
            pinnedImage.Remove();

            //Assert
            pinnedImage.ReleaseEvents().Should().ContainItemsAssignableTo<ImageRemovedDataEvent>();
        }
    }
}