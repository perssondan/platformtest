using System;
using System.Numerics;

namespace uwpPlatformer.Numerics
{
    public struct BoundingBox
    {
        public static readonly BoundingBox Empty = new BoundingBox(0, 0, 0, 0);

        public BoundingBox(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
        }

        public BoundingBox(float x, float y, float width, float height)
        {
            Position = new Vector2(x, y);
            Size = new Vector2(width, height);
        }

        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public float Width => Size.X;
        public float Height => Size.Y;
        public float Left => Position.X;
        public float Top => Position.Y;
        public float Right => Position.X + Width;
        public float Bottom => Position.Y + Height;
        public Vector2 Center => Position + (Size * 0.5f);
        public Vector2 TopRight => new Vector2(Top, Right);
        public Vector2 TopLeft => Position;
        public Vector2 BottomLeft => new Vector2(Left, Bottom);
        public Vector2 BottomRight => new Vector2(Right, Bottom);

        public BoundingBox Add(BoundingBox boundingBox)
        {
            return new BoundingBox(
                Left - boundingBox.Width * 0.5f,
                Top - boundingBox.Height * 0.5f,
                Width + boundingBox.Width,
                Height + boundingBox.Height);
        }

        public Vector2 Half()
        {
            return Size * 0.5f;
        }

        public BoundingBox Union(BoundingBox boundingBox)
        {
            var position = new Vector2(Math.Min(Left, boundingBox.Left), Math.Min(Top, boundingBox.Top));
            var minLeft = Math.Min(Left, boundingBox.Left);
            var minTop = Math.Min(Top, boundingBox.Top);
            var maxRight = Math.Max(Right, boundingBox.Right);
            var maxBottom = Math.Max(Bottom, boundingBox.Bottom);

            return new BoundingBox(new Vector2(minLeft, minTop), new Vector2(maxRight - minLeft, maxBottom - minTop));
        }

        public Vector2 BottomCenterOffset()
        {
            return new Vector2((float)Width / 2f, (float)Height);
        }
    }
}
