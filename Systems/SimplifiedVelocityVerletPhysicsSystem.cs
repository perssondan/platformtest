using GamesLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using uwpPlatformer.Components;
using uwpPlatformer.Extensions;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Systems
{
    public class SimplifiedVelocityVerletPhysicsSystem : SystemBase<SimplifiedVelocityVerletPhysicsSystem>
    {
        /// <summary>
        /// For reference, Simplified Velocity Verlet integration
        /// </summary>
        /// <param name="timingInfo"></param>
        public override void Update(TimingInfo timingInfo)
        {
            GameObjectManager.GameObjects
                .Select(gameObject => (gameObject, components: gameObject.GetComponents<PhysicsComponent, TransformComponent>()))
                .Where(result => result != default && result.components != default)
                .ToArray() // clone
                .ForEach(result =>
                {
                    Integrate(result.components.Item1, result.components.Item2, timingInfo.ElapsedTime);
                });
        }

        private void Integrate(PhysicsComponent physicsComponent,
                               TransformComponent transformComponent,
                               TimeSpan timeSpan)
        {
            var acceleration = ApplyForces(physicsComponent);
            var deltaTime = (float)timeSpan.TotalSeconds;

            var currentPosition = transformComponent.Position;
            var currentVelocity = transformComponent.Velocity;

            var newPosition = currentPosition + deltaTime * (currentVelocity + (deltaTime * acceleration * 0.5f));
            var newVelocity = currentVelocity + acceleration * deltaTime;

            transformComponent.Position = newPosition;
            transformComponent.Velocity = newVelocity;

            physicsComponent.ImpulseForce = Vector2.Zero;
        }

        private Vector2 ApplyForces(PhysicsComponent physicsComponent)
        {
            return (physicsComponent.Gravity + physicsComponent.ImpulseForce) * physicsComponent.MassInverted;
        }
    }
}
