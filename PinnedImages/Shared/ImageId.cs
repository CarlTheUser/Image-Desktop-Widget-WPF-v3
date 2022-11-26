namespace Shared
{
    public record struct ImageId(Guid Value)
    {
        public static ImageId New() => new(Value: Guid.NewGuid());

        public override string ToString()
        {
            return Value.ToString();
        }

        public static implicit operator Guid(ImageId id)
        {
            return id.Value;
        }
    }
}