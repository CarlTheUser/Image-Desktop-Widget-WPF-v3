namespace Infrastructure.Data
{
    //LiteDB needs parameterless constructors
    internal class FlattenedPinnedImageDataHolder
    {
        public Guid Id { get; set; }
        public string ImageDirectory { get; set; } = string.Empty;
        public string CaptionText { get; set; } = string.Empty;
        public bool CaptionVisible { get; set; }
        public double ShadowOpacity { get; set; }
        public double ShadowDepth { get; set; }
        public double ShadowDirection { get; set; }
        public double ShadowBlurRadius { get; set; }
        public bool ShadowVisible { get; set; }
        public double DimensionWidth { get; set; }
        public double DimensionHeight { get; set; }
        public double LocationX { get; set; }
        public double LocationY { get; set; }
        public string ImageColorHexValue { get; set; } = "#FFFFFF";
        public double FrameThicknessValue { get; set; }
        public double RotationAngle { get; set; }
        public double CornerRadius { get; set; }
        public bool IsShown { get; set; }
        public DateTime CreationTimestamp { get; set; }
    }
}