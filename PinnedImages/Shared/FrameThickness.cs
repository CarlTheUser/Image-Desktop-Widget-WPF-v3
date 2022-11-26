namespace Shared
{
    public record struct FrameThickness(double Value)
    {
        public static implicit operator double(FrameThickness frameThickness)
        {
            return frameThickness.Value;
        }
    }
}