using System.Numerics;
using uwpPlatformer.Constants;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Components
{
    public class PhysicsComponent : ComponentBase, IComponent
    {
        private float _mass;
        private Vector2 _acceleration;
        private Vector2 _velocity;

        public PhysicsComponent(GameObject gameObject) : base(gameObject)
        {
            Mass = 1f;
        }

        public Vector2 Velocity
        {
            get => _velocity;
            set
            {
                PreviousVelocity = _velocity;
                _velocity = value;
            }
        }

        public Vector2 Position { get; set; }

        public Vector2 PreviousVelocity { get; private set; }

        public Vector2 Acceleration
        {
            get => _acceleration;
            set
            {
                PreviousAcceleration = _acceleration;
                _acceleration = value;
            }
        }

        public Vector2 PreviousAcceleration { get; private set; }

        public Vector2 Gravity { get; set; } = new Vector2(0f, PlayerConstants.Gravity);

        public Vector2 LinearMomentum => Mass * Velocity;

        public float Drag { get; set; }

        public float Mass
        {
            get => _mass;
            set
            {
                if (_mass == value) return;

                _mass = value;
                MassInverted = 1f / _mass;
            }
        }

        public float MassInverted { get; private set; }

        public Vector2 ImpulseForce { get; set; }
    }
}
