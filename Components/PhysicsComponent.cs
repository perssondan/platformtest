using System.Numerics;
using uwpPlatformer.Constants;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Components
{
    public class PhysicsComponent : GameObjectComponent, IComponent
    {
        public PhysicsComponent(GameObject gameObject) : base(gameObject)
        {
            PhysicsComponentManager.Instance.AddComponent(this);
        }

        protected override void OnDispose()
        {
            PhysicsComponentManager.Instance.RemoveComponent(this);
        }

        public Vector2 OldVelocity { get; set; }
        public Vector2 OldPosition { get; set; }
        public Vector2 OldAcceleration { get; set; }

        public float Drag { get; set; }

        public Vector2 Gravity { get; set; } = new Vector2(0f, PlayerConstants.Gravity);

        public float Mass { get; set; } = 1f;

        public float MassInverted => 1f / Mass;

        public Vector2 ImpulseForce { get; set; }
    }
}
