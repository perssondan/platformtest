using System;
using System.Numerics;
using uwpKarate.GameObjects;

namespace uwpKarate.Components
{
    public class PhysicsComponent : IGameObjectComponent<World>
    {
        private readonly GameObject _gameObject;

        public PhysicsComponent(GameObject gameObject)
        {
            _gameObject = gameObject;
        }

        public void Update(World world, TimeSpan timeSpan)
        {
            var deltaTime = (float)timeSpan.TotalSeconds;
            // Apply gravity
            var newVelocity = _gameObject.TransformComponent.Velocity + (deltaTime * Gravity);
            // TODO: Limit velocity
            _gameObject.TransformComponent.Velocity = newVelocity;
            // TODO: is this right?
            _gameObject.TransformComponent.Position += (0.5f * (newVelocity + OldVelocity) * deltaTime);

            OldVelocity = _gameObject.TransformComponent.Velocity;
        }

        private Vector2 Force(Vector2 position)
        {
            return Gravity;
        }

        private Vector2 OldVelocity { get; set; }
        private Vector2 Acceleration { get; set; }

        private void DampHorizontalSpeed(float deltaTime)
        {
            _horizontalLerpTime += deltaTime;

            if (_gameObject.TransformComponent.Velocity.X != 0f)
            {
                var lerpVector = Vector2.Lerp(_gameObject.TransformComponent.Velocity * Vector2.UnitX, Vector2.Zero, _horizontalLerpTime);
                // damp or set horizontal velocity to zero
                _gameObject.TransformComponent.Velocity = lerpVector + (Vector2.UnitY * _gameObject.TransformComponent.Velocity);
            }
            else
            {
                _horizontalLerpTime = 0f;
            }
        }

        private float _horizontalLerpTime = 0f;

        public Vector2 Gravity { get; set; }
        public Vector2 MaximumVelocity { get; set; }// TODO: Implement limit
    }
}