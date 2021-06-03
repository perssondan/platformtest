using GamesLibrary.Models;
using System;
using System.Linq;
using System.Numerics;
using uwpPlatformer.Components;
using uwpPlatformer.Extensions;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Systems
{
    public class VelocityVerletPhysicsSystem : SystemBase<VelocityVerletPhysicsSystem>
    {
        public VelocityVerletPhysicsSystem()
        {
        }

        public override void Update(TimingInfo timingInfo)
        {
            GameObjectManager.GameObjects
                .Select(gameObject => (gameObject, gameObject.GetComponents<PhysicsComponent, TransformComponent>()))
                .Where(result => result != default && result.Item2 != default)
                .ToArray() // clone
                .ForEach(result =>
                {
                    Integrate(result.Item2.Item1, result.Item2.Item2, timingInfo.ElapsedTime);
                });
        }

        private void Integrate(PhysicsComponent physicsComponent,
                               TransformComponent transformComponent,
                               TimeSpan timeSpan)
        {
            var deltaTime = (float)timeSpan.TotalSeconds;

            var newPosition = transformComponent.Position + (transformComponent.Velocity * deltaTime) + (physicsComponent.Acceleration * (deltaTime * deltaTime * 0.5f));
            var newAcceleration = ApplyForces(physicsComponent);
            var newVelocity = transformComponent.Velocity + ((physicsComponent.Acceleration + newAcceleration) * (deltaTime * 0.5f));

            physicsComponent.Acceleration = newAcceleration;
            transformComponent.Velocity = newVelocity;
            transformComponent.Position = newPosition;
        }

        private Vector2 ApplyForces(PhysicsComponent physicsComponent)
        {
            return physicsComponent.Gravity;
        }
    }
}
