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
    public class PositionVerletSystem : SystemBase<PositionVerletSystem>
    {
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
            physicsComponent.OldAcceleration = physicsComponent.Acceleration;

            var acceleration = ApplyForces(physicsComponent);

            var deltaTime = (float)timeSpan.TotalSeconds;

            var newPosition = transformComponent.Position + (transformComponent.Position - physicsComponent.OldPosition) + (acceleration * deltaTime * deltaTime);
            physicsComponent.OldPosition = transformComponent.Position;
            transformComponent.Position = newPosition;

            physicsComponent.Acceleration = Vector2.Zero;
        }

        private Vector2 ApplyForces(PhysicsComponent physicsComponent)
        {
            return (physicsComponent.Gravity + physicsComponent.AdditionalForce) * physicsComponent.MassInverted;
        }
    }
}
