using Windows.UI;

namespace uwpPlatformer.Extensions
{
    public static class ColorExtensions
    {
        public static Color Lerp(this Color startColor, Color endColor, float percentage)
        {
            var alpha = Lerp(startColor.A, endColor.A, percentage);
            var red = Lerp(startColor.R, endColor.R, percentage);
            var green = Lerp(startColor.G, endColor.G, percentage);
            var blue = Lerp(startColor.B, endColor.B, percentage);

            return Color.FromArgb((byte)alpha, (byte)red, (byte)green, (byte)blue);
        }

        private static float Lerp(float startValue, float endValue, float by)
        {
            return startValue * (1 - by) + (endValue * by);
        }
    }
}
