using System;
using System.Numerics;

namespace uwpPlatformer.Extensions
{
    public static class Vector2Extensions
    {
        public static float GetByIndex(this Vector2 vector2, int index) => index == 0 ? vector2.X : vector2.Y;

        public static void Swap(ref Vector2 first, ref Vector2 second)
        {
            var temp = first;
            first = second;
            second = temp;
        }

        public static Vector2 Normalize(this Vector2 source)
        {
            return Vector2.Normalize(source);
        }

        public static Vector2 VectorFromAngle(this float angle)
        {
            return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
        }
    }
}