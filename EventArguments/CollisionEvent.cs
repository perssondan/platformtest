using uwpPlatformer.Components;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Events
{
    public struct CollisionEvent
    {
        public CollisionEvent(GameObject gameObject, GameObject isCollidingWith, CollisionInfo collisionInfo)
        {
            GameObject = gameObject;
            IsCollidingWith = isCollidingWith;
            CollisionInfo = collisionInfo;
        }

        public readonly GameObject GameObject;
        public readonly GameObject IsCollidingWith;
        public readonly CollisionInfo CollisionInfo;
    }
}
