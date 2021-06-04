using GamesLibrary.Models;
using System.Linq;
using System.Numerics;
using uwpPlatformer.Components;
using uwpPlatformer.Extensions;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Systems
{
    /// <summary>
    /// For reference, Velocity Verlet integration
    /// </summary>
    /// <remarks>
    /// Due to the nature of <see cref="MoveSystem"/>, this won't work in our game
    /// </remarks>
    public class VelocityVerletPhysicsSystem : SystemBase<VelocityVerletPhysicsSystem>
    {
        public override void Update(TimingInfo timingInfo)
        {
            var deltaTime = (float)timingInfo.ElapsedTime.TotalSeconds;

            GameObjectManager.GameObjects
                .Select(gameObject => (gameObject, components: gameObject.GetComponents<PhysicsComponent, TransformComponent>()))
                .Where(result => result != default && result.Item2 != default)
                .ToArray() // clone
                .ForEach(result =>
                {
                    Integrate(result.components.Item1, result.components.Item2, deltaTime);
                });
        }

        private void Integrate(PhysicsComponent physicsComponent,
                               TransformComponent transformComponent,
                               float deltaTime)
        {
            var currentPosition = transformComponent.Position;
            var currentVelocity = transformComponent.Velocity;
            // N.B 1. If acceleration is constant in regards to the position and time, we can omit one call to ApplyForces
            // and use the same force in both calculations
            // N.B 2. Store the first acceleration to be used in the next frame to save the extra calculation
            var acceleration = ApplyForces(physicsComponent, currentPosition, deltaTime);

            var newPosition = currentPosition + (currentVelocity * deltaTime) + (acceleration * (deltaTime * deltaTime * 0.5f));
            var newAcceleration = ApplyForces(physicsComponent, newPosition, deltaTime * 2);
            var newVelocity = currentVelocity + ((acceleration + newAcceleration) * (deltaTime * 0.5f));

            transformComponent.Velocity = newVelocity;
            transformComponent.Position = newPosition;

            physicsComponent.ImpulseForce = Vector2.Zero;
        }

        private Vector2 ApplyForces(PhysicsComponent physicsComponent, Vector2 currentPosition, float deltaTime)
        {
            return (physicsComponent.Gravity + physicsComponent.ImpulseForce) * physicsComponent.MassInverted;
        }
    }
}
