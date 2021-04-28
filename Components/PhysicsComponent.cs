using System;
using System.Numerics;
using uwpKarate.GameObjects;

namespace uwpKarate.Components
{
    public class PhysicsComponent : IGameObjectComponent<World>
    {
        private readonly GameObject _gameObject;
        private Action<World, TimeSpan> _integrateFunc;
        private IntegrationType _integration;

        public PhysicsComponent(GameObject gameObject)
        {
            _gameObject = gameObject;
            Integration = IntegrationType.VelocityVerlet;
        }

        public Vector2 OldVelocity { get; private set; }
        public Vector2 OldPosition { get; private set; }
        public Vector2 Drag { get; set; }
        public Vector2 Acceleration { get; private set; }
        public Vector2 Gravity { get; set; }
        public Vector2 MaximumVelocity { get; set; }// TODO: Implement limit

        public IntegrationType Integration
        {
            get => _integration;
            set
            {
                if (_integration == value) return;
                _integration = value;
                ChangeIntegration(_integration);
            }
        }

        public void Update(World world, TimeSpan timeSpan)
        {
            _integrateFunc?.Invoke(world, timeSpan);
        }

        private void ExplicitEulerIntegration(World world, TimeSpan timeSpan)
        {
            var acceleration = ApplyForces();
            var deltaTime = (float)timeSpan.TotalSeconds;
            OldPosition = _gameObject.TransformComponent.Position;
            _gameObject.TransformComponent.Position += (deltaTime * _gameObject.TransformComponent.Velocity);
            OldVelocity = _gameObject.TransformComponent.Velocity;
            _gameObject.TransformComponent.Velocity += (deltaTime * acceleration);
            Acceleration = acceleration;
    }

        private void SemiImplicitEulerIntegration(World world, TimeSpan timeSpan)
        {
            var acceleration = ApplyForces();
            var deltaTime = (float)timeSpan.TotalSeconds;
            OldVelocity = _gameObject.TransformComponent.Velocity;
            _gameObject.TransformComponent.Velocity += (deltaTime * acceleration);
            OldPosition = _gameObject.TransformComponent.Position;
            _gameObject.TransformComponent.Position += (deltaTime * _gameObject.TransformComponent.Velocity);
            Acceleration = acceleration;
        }

        private void SimplifiedVelocityVerletIntegration(World world, TimeSpan timeSpan)
        {
            var acceleration = ApplyForces();
            var deltaTime = (float)timeSpan.TotalSeconds;
            // Apply acceleration
            var newVelocity = _gameObject.TransformComponent.Velocity + (deltaTime * acceleration);
            // TODO: Limit velocity
            _gameObject.TransformComponent.Velocity = newVelocity;

            OldPosition = _gameObject.TransformComponent.Position;
            _gameObject.TransformComponent.Position += (0.5f * (newVelocity + OldVelocity) * deltaTime);

            OldVelocity = _gameObject.TransformComponent.Velocity;
            Acceleration = acceleration;
        }

        private void PositionVerletIntegration(World world, TimeSpan timeSpan)
        {
            var deltaTime = (float)timeSpan.TotalSeconds;
            var currentPosition = _gameObject.TransformComponent.Position;
            var acceleration = ApplyForces();
            _gameObject.TransformComponent.Position += (_gameObject.TransformComponent.Position - OldPosition) + (acceleration * deltaTime * deltaTime);
            OldPosition = currentPosition;
            Acceleration = acceleration;
        }

        private void VelocityVerletIntegration(World world, TimeSpan timeSpan)
        {
            var deltaTime = (float)timeSpan.TotalSeconds;
            var currentPosition = _gameObject.TransformComponent.Position;
            var newPosition = currentPosition + (_gameObject.TransformComponent.Velocity * deltaTime) + (Acceleration * deltaTime * deltaTime * 0.5f);
            var acceleration = ApplyForces();
            var newVelocity = _gameObject.TransformComponent.Velocity + ((Acceleration + acceleration) * (deltaTime * 0.5f));

            Vector2 resolvedPosition = Vector2.Zero;
            if (_gameObject?.ColliderComponent.IsCollidingTest(OldPosition, newPosition, out resolvedPosition) == true)
            {
                var resolveVector = Vector2.One;
                if (resolvedPosition.X != newPosition.X)
                {
                    resolveVector *= Vector2.UnitY;
                    newVelocity *= Vector2.UnitY;
                }
                if (resolvedPosition.Y != newPosition.Y)
                {
                    resolveVector *= Vector2.UnitX;
                    newVelocity *= Vector2.UnitX;
                }
                newPosition = resolvedPosition;
                
                //newVelocity = OldVelocity * resolveVector;
                //acceleration *= resolveVector;
            }

            if (Math.Abs(newVelocity.X) < 5f)
            {
                newVelocity.X = 0f;
            }

            OldPosition = newPosition;
            OldVelocity = newVelocity;
            Acceleration = acceleration;

            _gameObject.TransformComponent.Position = newPosition;
            _gameObject.TransformComponent.Velocity = newVelocity;
        }

        private void ChangeIntegration(IntegrationType integration)
        {
            switch (integration)
            {
                case IntegrationType.ExplicitEuler:
                    _integrateFunc = ExplicitEulerIntegration;
                    break;
                case IntegrationType.SemiImplicitEuler:
                    _integrateFunc = SemiImplicitEulerIntegration;
                    break;
                case IntegrationType.PositionVerlet:
                    _integrateFunc = PositionVerletIntegration;
                    break;
                case IntegrationType.VelocityVerlet:
                    _integrateFunc = VelocityVerletIntegration;
                    break;
                case IntegrationType.SimplifiedVelocityVerlet:
                    _integrateFunc = SimplifiedVelocityVerletIntegration;
                    break;
            }
        }

        private void ResolveConstraints(World world, float deltaTime)
        {
            // TODO: Collisions
            if (world.TryGetGroundedTile(_gameObject, out _, deltaTime))
            {

            }
        }

        private Vector2 ApplyForces()
        {
            var gravityForce = Gravity;
            var dragForce = 0.5f * Drag * (_gameObject.TransformComponent.Velocity.Length() * _gameObject.TransformComponent.Velocity);
            return gravityForce - dragForce;
        }

        private void DragSample()
        {
            var factor = .99f;
            var velocity = 2f;
            var drag = factor * (velocity * velocity);
        }

        public enum IntegrationType
        {
            None,
            ExplicitEuler,
            SemiImplicitEuler,
            PositionVerlet,
            VelocityVerlet,
            SimplifiedVelocityVerlet,
        }
    }
}