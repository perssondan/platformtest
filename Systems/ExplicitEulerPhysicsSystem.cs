﻿using GamesLibrary.Models;
using System.Linq;
using System.Numerics;
using uwpPlatformer.Components;
using uwpPlatformer.Extensions;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Systems
{
    /// <summary>
    /// For reference, Explicit Euler integration
    /// </summary>
    public class ExplicitEulerPhysicsSystem : SystemBase<ExplicitEulerPhysicsSystem>
    {
        public override void Update(TimingInfo timingInfo)
        {
            var deltaTime = (float)timingInfo.ElapsedTime.TotalSeconds;

            GameObjectManager.GameObjects
                .Select(gameObject => (gameObject, components: gameObject.GetComponents<PhysicsComponent, TransformComponent>()))
                .Where(result => result != default && result.components != default)
                .ToArray() // clone
                .ForEach(result =>
                {
                    Integrate(result.components.Item1, result.components.Item2, deltaTime);
                });
        }

        private void Integrate(PhysicsComponent physicsComponent,
                               TransformComponent transform,
                               float deltaTime)
        {
            var newAcceleration = ApplyForces(physicsComponent);

            var currentPosition = transform.Position;
            var currentVelocity = transform.Velocity;
            var currentAcceleration = physicsComponent.OldAcceleration;

            var newPosition = currentPosition + (deltaTime * currentVelocity);
            var newVelocity = currentVelocity + (deltaTime * currentAcceleration);

            transform.Position = newPosition;
            transform.Velocity = newVelocity;
            physicsComponent.OldAcceleration = newAcceleration;

            physicsComponent.ImpulseForce = Vector2.Zero;
        }

        private Vector2 ApplyForces(PhysicsComponent physicsComponent)
        {
            return (physicsComponent.Gravity + physicsComponent.ImpulseForce) * physicsComponent.MassInverted;
        }
    }
}
