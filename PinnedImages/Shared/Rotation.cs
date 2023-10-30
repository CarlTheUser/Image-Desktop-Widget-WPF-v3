namespace Shared
{
    public record struct Rotation(double Angle)
    {
        public static readonly Rotation Zero = new(0);

        public static implicit operator double(Rotation rotation)
        {
            return rotation.Angle;
        }
    }
}