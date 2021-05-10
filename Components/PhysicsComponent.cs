using System.Numerics;
using uwpKarate.Constants;
using uwpKarate.GameObjects;

namespace uwpKarate.Components
{
    public class PhysicsComponent : GameObjectComponent, IGameObjectComponent
    {
        public PhysicsComponent(GameObject gameObject) : base(gameObject)
        {
            PhysicsComponentManager.Instance.AddComponent(this);
        }

        public Vector2 OldVelocity { get; set; }
        public Vector2 OldPosition { get; set; }
        public Vector2 Drag { get; set; }
        public Vector2 Acceleration { get; set; }
        public Vector2 Gravity { get; set; } = new Vector2(0f, PlayerConstants.Gravity);
    }
}