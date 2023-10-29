using GamesLibrary.Models;
using System;
using System.Linq;
using System.Numerics;
using uwpPlatformer.Components;
using uwpPlatformer.Extensions;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Systems
{
    /// <summary>
    /// Physics system that integrates with Position Verlet.
    /// </summary>
    /// <remarks>
    /// Due to the nature of <see cref="TranslateTransformSystem"/>,
    /// we need to estimate the actual velocity, var estimatedVelocity = (newPosition-currentPositon) * 0.5f;,
    /// to make this to work in the specific setup of this game.
    /// However, the <see cref="PositionVerletPhysicsSystem"/> is added here for reference only.
    /// </remarks>
    public class PositionVerletPhysicsSystem : ISystem
    {
        private readonly IGameObjectManager _gameObjectManager;

        public PositionVerletPhysicsSystem(IGameObjectManager gameObjectManager)
        {
            _gameObjectManager = gameObjectManager;
        }

        public string Name => nameof(PositionVerletPhysicsSystem);

        public void Init()
        {
        }

        public void Update(TimingInfo timingInfo)
        {
            _gameObjectManager.GameObjects
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
            var acceleration = ApplyForces(physicsComponent);

            var deltaTime = (float)timeSpan.TotalSeconds;

            var currentPosition = transformComponent.Position;
            var previousPosition = transformComponent.PreviousPosition;

            var newPosition = currentPosition + (currentPosition - previousPosition) + (acceleration * deltaTime * deltaTime);

            transformComponent.Position = newPosition;

            physicsComponent.ImpulseForce = Vector2.Zero;
        }

        private Vector2 ApplyForces(PhysicsComponent physicsComponent)
        {
            return (physicsComponent.Gravity + physicsComponent.ImpulseForce) * physicsComponent.MassInverted;
        }
    }
}
