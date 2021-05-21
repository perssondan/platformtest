using uwpPlatformer.Components;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Events
{
    public class CollisionArgument
    {
        public GameObject GameObject { get; set; }
        public GameObject IsCollidingWith { get; set; }
        public CollisionInfo CollisionInfo { get; set; }
    }
}
