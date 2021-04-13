using System;
using System.Numerics;
using uwpKarate.GameObjects;

namespace uwpKarate.Components
{
    public class PhysicsComponent
    {
        private readonly GameObject _gameObject;

        public PhysicsComponent(GameObject gameObject)
        {
            _gameObject = gameObject;

            Gravity = new Vector2(0f, 250f);
        }

        public void Update(World world, TimeSpan timeSpan)
        {
            var velocityToAdd = (float)timeSpan.TotalSeconds * Gravity;
            var isOnGround = world.IsOnGround(_gameObject);
            if (isOnGround && !IsJumping)
            {
                // if we're on ground, don't apply more gravity
                _gameObject.TransformComponent.Velocity *= Vector2.UnitX;
            }
            else
            {
                _gameObject.TransformComponent.Velocity += velocityToAdd;
            }
        }

        public Vector2 Gravity { get; private set; }

        private bool IsJumping => _gameObject.TransformComponent.Velocity.Y < 0f;

        private bool IsFalling => _gameObject.TransformComponent.Velocity.Y > 0f;
    }
}