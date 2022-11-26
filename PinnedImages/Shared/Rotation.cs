namespace Shared
{
    public record struct Rotation(double Angle)
    {
        public static implicit operator double(Rotation rotation)
        {
            return rotation.Angle;
        }
    }
}