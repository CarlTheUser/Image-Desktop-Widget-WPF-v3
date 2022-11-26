namespace Shared
{
    public record Shadow(
            double Opacity,
            double Depth,
            double Direction,
            double BlurRadius,
            bool IsVisible)
    {
        public static readonly Shadow None = new(
            Opacity: 0,
            Depth: 0,
            Direction: 0,
            BlurRadius: 0,
            IsVisible: false);
    }
}