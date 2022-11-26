namespace Shared
{
    public record struct Corner(double Radius)
    {
        public static readonly Corner None = new(Radius: 0);

        public static implicit operator double(Corner corner)
        {
            return corner.Radius;
        }
    }
}