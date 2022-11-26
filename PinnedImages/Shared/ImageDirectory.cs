namespace Shared
{
    public record ImageDirectory(string Path)
    {
        public override string ToString()
        {
            return Path;
        }

        public static implicit operator string(ImageDirectory imageDirectory)
        {
            return imageDirectory.Path;
        }
    }
}