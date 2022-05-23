using uwpPlatformer.Numerics;
using Windows.Foundation;

namespace uwpPlatformer.Extensions
{
    public static class BoundingBoxExtensions
    {
        public static Rect ToRect(this BoundingBox boundingBox)
        {
            return new Rect(boundingBox.Left, boundingBox.Top, boundingBox.Width, boundingBox.Height);
        }
    }
}
