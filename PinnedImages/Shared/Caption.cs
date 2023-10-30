namespace Shared
{
    public record Caption(string Text, bool IsVisible = true)
    {
        public static readonly Caption None = new(Text: string.Empty, IsVisible: false);
    }
}