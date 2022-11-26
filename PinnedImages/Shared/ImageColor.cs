﻿namespace Shared
{
    public record struct ImageColor(string HexValue)
    {
        public override string ToString()
        {
            return HexValue;
        }

        public static implicit operator string(ImageColor imageColor)
        {
            return imageColor.HexValue;
        }
    }
}