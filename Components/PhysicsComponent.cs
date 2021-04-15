using System;
using System.Numerics;
using uwpKarate.GameObjects;

namespace uwpKarate.Components
{
    public class PhysicsComponent
    {
        private readonly GameObject _gameObject;
        private Vector2 _previousAppliedVelocity = Vector2.Zero;
        private float _maxVelocity = 200f;

        public PhysicsComponent(GameObject gameObject)
        {
            _gameObject = gameObject;

            Gravity = new Vector2(0f, 56.8f);
        }

        public void Update(World world, TimeSpan timeSpan)
        {
            // velocity should increase over time when gravity is applied (no impulse)
            var velocityToAdd = _previousAppliedVelocity + (float)timeSpan.TotalSeconds * Gravity;
            var velocity = velocityToAdd.Length();
            if (velocity > _maxVelocity)
            {
                var factor = _maxVelocity / velocity;
                velocityToAdd *= factor;
            }
            // snap to ground
            if (world.TryGetGroundedTile(_gameObject, out var tileGameObject))
            {
                _gameObject.TransformComponent.Position *= Vector2.UnitX;
                _gameObject.TransformComponent.Position += (Vector2.UnitY * tileGameObject.TransformComponent.Position) - (32f * Vector2.UnitY);
                velocityToAdd = Vector2.Zero;
            }

            //if (!IsJumping)
            //{
            //    velocityToAdd = Vector2.Zero;
            //}

            _gameObject.TransformComponent.Velocity += velocityToAdd;
            _previousAppliedVelocity = velocityToAdd;
        }

        public Vector2 Gravity { get; private set; }

        private bool IsJumping => _gameObject.TransformComponent.Velocity.Y < 0f;

        private bool IsFalling => _gameObject.TransformComponent.Velocity.Y > 0f;
    }
}