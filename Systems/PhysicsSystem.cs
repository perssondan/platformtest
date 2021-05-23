using GamesLibrary.Systems;
using System;
using System.Numerics;
using uwpPlatformer.Components;

namespace uwpPlatformer.Systems
{
    public class PhysicsSystem : SystemBase<PhysicsSystem>
    {
        private Action<PhysicsComponent, TimeSpan> _integrateFunc;
        private IntegrationType _integration;
        private readonly IEventSystem _eventSystem;

        public PhysicsSystem(IEventSystem eventSystem)
        {
            _eventSystem = eventSystem;
            Integration = IntegrationType.VelocityVerlet;
        }

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

        public override void Update(TimeSpan deltaTime)
        {
            foreach (var physicsComponent in PhysicsComponentManager.Instance.Components)
            {
                _integrateFunc?.Invoke(physicsComponent, deltaTime);
            }
        }

        private Vector2 ApplyForces(PhysicsComponent physicsComponent)
        {
            var gravityForce = physicsComponent.Gravity;
            var dragForce = 0.5f * physicsComponent.Drag * (physicsComponent.GameObject.TransformComponent.Velocity.Length() * physicsComponent.GameObject.TransformComponent.Velocity);
            return gravityForce - dragForce;
        }

        private void ExplicitEulerIntegration(PhysicsComponent physicsComponent, TimeSpan timeSpan)
        {
            var acceleration = ApplyForces(physicsComponent);
            var deltaTime = (float)timeSpan.TotalSeconds;
            physicsComponent.OldPosition = physicsComponent.GameObject.TransformComponent.Position;
            //GameObject.TransformComponent.Position += (deltaTime * GameObject.TransformComponent.Velocity);
            physicsComponent.OldVelocity = physicsComponent.GameObject.TransformComponent.Velocity;
            physicsComponent.GameObject.TransformComponent.Velocity += (deltaTime * acceleration);
            physicsComponent.Acceleration = acceleration;
        }

        private void SemiImplicitEulerIntegration(PhysicsComponent physicsComponent, TimeSpan timeSpan)
        {
            var acceleration = ApplyForces(physicsComponent);
            var deltaTime = (float)timeSpan.TotalSeconds;
            physicsComponent.OldVelocity = physicsComponent.GameObject.TransformComponent.Velocity;
            physicsComponent.GameObject.TransformComponent.Velocity += (deltaTime * acceleration);
            physicsComponent.OldPosition = physicsComponent.GameObject.TransformComponent.Position;
            //GameObject.TransformComponent.Position += (deltaTime * GameObject.TransformComponent.Velocity);
            physicsComponent.Acceleration = acceleration;
        }

        private void SimplifiedVelocityVerletIntegration(PhysicsComponent physicsComponent, TimeSpan timeSpan)
        {
            var acceleration = ApplyForces(physicsComponent);
            var deltaTime = (float)timeSpan.TotalSeconds;
            // Apply acceleration
            var newVelocity = physicsComponent.GameObject.TransformComponent.Velocity + (deltaTime * acceleration);
            // TODO: Limit velocity
            physicsComponent.GameObject.TransformComponent.Velocity = newVelocity;

            physicsComponent.OldPosition = physicsComponent.GameObject.TransformComponent.Position;
            //GameObject.TransformComponent.Position += (0.5f * (newVelocity + OldVelocity) * deltaTime);

            physicsComponent.OldVelocity = physicsComponent.GameObject.TransformComponent.Velocity;
            physicsComponent.Acceleration = acceleration;
        }

        private void PositionVerletIntegration(PhysicsComponent physicsComponent, TimeSpan timeSpan)
        {
            //var deltaTime = (float)timeSpan.TotalSeconds;
            //var currentPosition = physicsComponent.GameObject.TransformComponent.Position;
            //var acceleration = ApplyForces(physicsComponent);
            //physicsComponent.GameObject.TransformComponent.Position += (physicsComponent.GameObject.TransformComponent.Position - physicsComponent.OldPosition) + (acceleration * deltaTime * deltaTime);
            //physicsComponent.OldPosition = currentPosition;
            //physicsComponent.Acceleration = acceleration;
        }

        private void VelocityVerletIntegration(PhysicsComponent physicsComponent, TimeSpan timeSpan)
        {
            var deltaTime = (float)timeSpan.TotalSeconds;
            //var oldPosition = physicsComponent.GameObject.TransformComponent.Position;
            //var newPosition = oldPosition + (physicsComponent.GameObject.TransformComponent.Velocity * deltaTime) + (physicsComponent.Acceleration * deltaTime * deltaTime * 0.5f);
            var acceleration = ApplyForces(physicsComponent);
            var newVelocity = physicsComponent.GameObject.TransformComponent.Velocity + ((physicsComponent.Acceleration + acceleration) * (deltaTime * 0.5f));
            //var pos = oldPosition + (newVelocity * deltaTime);

            if (Math.Abs(newVelocity.X) < 5f)
            {
                newVelocity.X = 0f;
            }

            physicsComponent.Acceleration = acceleration;
            physicsComponent.GameObject.TransformComponent.Velocity = newVelocity;
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
