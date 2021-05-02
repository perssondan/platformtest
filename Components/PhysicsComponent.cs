using System;
using System.Numerics;
using uwpKarate.GameObjects;

namespace uwpKarate.Components
{
    public class PhysicsComponent : GameObjectComponent, IGameObjectComponent<World>
    {
        private Action<World, TimeSpan> _integrateFunc;
        private IntegrationType _integration;

        public PhysicsComponent(GameObject gameObject) : base(gameObject)
        {
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
            OldPosition = GameObject.TransformComponent.Position;
            GameObject.TransformComponent.Position += (deltaTime * GameObject.TransformComponent.Velocity);
            OldVelocity = GameObject.TransformComponent.Velocity;
            GameObject.TransformComponent.Velocity += (deltaTime * acceleration);
            Acceleration = acceleration;
    }

        private void SemiImplicitEulerIntegration(World world, TimeSpan timeSpan)
        {
            var acceleration = ApplyForces();
            var deltaTime = (float)timeSpan.TotalSeconds;
            OldVelocity = GameObject.TransformComponent.Velocity;
            GameObject.TransformComponent.Velocity += (deltaTime * acceleration);
            OldPosition = GameObject.TransformComponent.Position;
            GameObject.TransformComponent.Position += (deltaTime * GameObject.TransformComponent.Velocity);
            Acceleration = acceleration;
        }

        private void SimplifiedVelocityVerletIntegration(World world, TimeSpan timeSpan)
        {
            var acceleration = ApplyForces();
            var deltaTime = (float)timeSpan.TotalSeconds;
            // Apply acceleration
            var newVelocity = GameObject.TransformComponent.Velocity + (deltaTime * acceleration);
            // TODO: Limit velocity
            GameObject.TransformComponent.Velocity = newVelocity;

            OldPosition = GameObject.TransformComponent.Position;
            GameObject.TransformComponent.Position += (0.5f * (newVelocity + OldVelocity) * deltaTime);

            OldVelocity = GameObject.TransformComponent.Velocity;
            Acceleration = acceleration;
        }

        private void PositionVerletIntegration(World world, TimeSpan timeSpan)
        {
            var deltaTime = (float)timeSpan.TotalSeconds;
            var currentPosition = GameObject.TransformComponent.Position;
            var acceleration = ApplyForces();
            GameObject.TransformComponent.Position += (GameObject.TransformComponent.Position - OldPosition) + (acceleration * deltaTime * deltaTime);
            OldPosition = currentPosition;
            Acceleration = acceleration;
        }

        private void VelocityVerletIntegration(World world, TimeSpan timeSpan)
        {
            var deltaTime = (float)timeSpan.TotalSeconds;
            var currentPosition = GameObject.TransformComponent.Position;
            var newPosition = currentPosition + (GameObject.TransformComponent.Velocity * deltaTime) + (Acceleration * deltaTime * deltaTime * 0.5f);
            var acceleration = ApplyForces();
            var newVelocity = GameObject.TransformComponent.Velocity + ((Acceleration + acceleration) * (deltaTime * 0.5f));

            Vector2 resolvedPosition = Vector2.Zero;
            if (GameObject?.ColliderComponent.IsCollidingTest(OldPosition, newPosition, out resolvedPosition) == true)
            {
                if (resolvedPosition.X != newPosition.X)
                {
                    newVelocity *= Vector2.UnitY;
                }
                if (resolvedPosition.Y != newPosition.Y)
                {
                    newVelocity *= Vector2.UnitX;
                }
                newPosition = resolvedPosition;
            }

            if (Math.Abs(newVelocity.X) < 5f)
            {
                newVelocity.X = 0f;
            }

            OldPosition = newPosition;
            OldVelocity = newVelocity;
            Acceleration = acceleration;

            GameObject.TransformComponent.Position = newPosition;
            GameObject.TransformComponent.Velocity = newVelocity;
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

        private Vector2 ApplyForces()
        {
            var gravityForce = Gravity;
            var dragForce = 0.5f * Drag * (GameObject.TransformComponent.Velocity.Length() * GameObject.TransformComponent.Velocity);
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