using System.Numerics;
using Windows.Foundation;

namespace uwpPlatformer.Extensions
{
    public static class RectExtensions
    {
        public static Vector2 TopLeft(this Rect rect)
        {
            return new Vector2((float)rect.Left, (float)rect.Top);
        }

        public static Vector2 TopRight(this Rect rect)
        {
            return new Vector2((float)rect.Right, (float)rect.Top);
        }

        public static Vector2 BottomLeft(this Rect rect)
        {
            return new Vector2((float)rect.Left, (float)rect.Bottom);
        }

        public static Vector2 BottomRight(this Rect rect)
        {
            return new Vector2((float)rect.Right, (float)rect.Bottom);
        }

        public static Vector2 BottomCenter(this Rect rect)
        {
            return new Vector2((float)rect.Right - (float)(rect.Width / 2f), (float)rect.Bottom);
        }

        public static Vector2 BottomCenterOffset(this Rect rect)
        {
            return new Vector2((float)rect.Width/2f, (float)rect.Height);
        }

        public static Vector2 Size(this Rect rect)
        {
            return new Vector2((float)rect.Width, (float)rect.Height);
        }

        public static Rect Add(this Rect target, Size source)
        {
            return new Rect(
                target.X - source.Width / 2f,
                target.Y - source.Height / 2f,
                target.Width + source.Width,
                target.Height + source.Height);
        }

        public static Rect Add(this Rect target, Vector2 source)
        {
            return target.Add(source.ToSize());
        }
    }
}
