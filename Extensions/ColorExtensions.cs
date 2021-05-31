using GamesLibrary.Utilities;
using Windows.UI;

namespace uwpPlatformer.Extensions
{
    public static class ColorExtensions
    {
        public static Color Lerp(this Color startColor, Color endColor, float percentage)
        {
            var alpha = GameMath.Lerp(startColor.A, endColor.A, percentage);
            var red = GameMath.Lerp(startColor.R, endColor.R, percentage);
            var green = GameMath.Lerp(startColor.G, endColor.G, percentage);
            var blue = GameMath.Lerp(startColor.B, endColor.B, percentage);

            return Color.FromArgb((byte)alpha, (byte)red, (byte)green, (byte)blue);
        }
    }
}
