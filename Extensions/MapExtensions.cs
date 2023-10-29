using uwpPlatformer.Models;
using uwpPlatformer.Numerics;

namespace uwpPlatformer.Extensions
{
    public static class MapExtensions
    {
        public static BoundingBox ToBoundingBox(this Map map)
        {
            return new BoundingBox(0, 0, map.MapWidth, map.MapHeight);
        }
    }
}
