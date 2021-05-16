using uwpKarate.Components;
using uwpKarate.GameObjects;

namespace uwpKarate.Events
{
    public class CollisionArgument
    {
        public GameObject GameObject { get; set; }
        public GameObject IsCollidingWith { get; set; }
        public CollisionInfo CollisionInfo { get; set; }
    }
}
