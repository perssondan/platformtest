using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace uwpKarate.Extensions
{
    public static class RectExtensions
    {
        public static Vector2 Pos(this Rect rect)
        {
            return new Vector2((float)rect.X, (float)rect.Y);
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
