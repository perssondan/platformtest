using uwpPlatformer.Components;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Events
{
    public struct CollisionEvent
    {
        public CollisionEvent(GameObject gameObject, GameObject isCollidingWith, CollisionManifold collisionManifold)
        {
            GameObject = gameObject;
            IsCollidingWith = isCollidingWith;
            CollisionManifold = collisionManifold;
        }

        public readonly GameObject GameObject;
        public readonly GameObject IsCollidingWith;
        public readonly CollisionManifold CollisionManifold;
    }
}
