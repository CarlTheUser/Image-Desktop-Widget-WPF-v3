namespace Shared
{
    public record Caption(string Text, bool IsVisible)
    {
        public static readonly Caption None = new(Text: string.Empty, IsVisible: false);
    }
}