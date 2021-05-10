using System.Numerics;
using Windows.Foundation;

namespace uwpKarate.Extensions
{
    public static class Vector2Extensions
    {
        public static void Swap(ref Vector2 first, ref Vector2 second)
        {
            var temp = first;
            first = second;
            second = temp;
        }

        public static Rect ToRect(this Vector2 vector2)
        {
            return new Rect(vector2.ToPoint(), new Point());
        }

        public static Rect ToRect(this Vector2 vector2, Size size)
        {
            return new Rect(vector2.ToPoint(), size);
        }

        public static Rect ToRect(this Vector2 vector2, float width, float height)
        {
            return new Rect(vector2.ToPoint(), new Size(width, height));
        }

        public static Rect ToRect(this Vector2 topLeft, Vector2 bottomRight)
        {
            return new Rect(topLeft.ToPoint(), bottomRight.ToSize());
        }

        public static Vector2 Normalize(this Vector2 source)
        {
            var reverseLenght = 1f / source.Length();
            return reverseLenght * source;
        }
    }
}