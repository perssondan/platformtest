using GamesLibrary.Models;
using System.Linq;
using System.Numerics;
using uwpPlatformer.Components;
using uwpPlatformer.Extensions;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Systems
{
    /// <summary>
    /// For reference, Semi-implicit Euler integration
    /// </summary>
    public class SemiImplicitEulerPhysicsSystem : ISystem
    {
        private readonly IGameObjectManager _gameObjectManager;

        public SemiImplicitEulerPhysicsSystem(IGameObjectManager gameObjectManager)
        {
            _gameObjectManager = gameObjectManager;
        }

        public string Name => nameof(SemiImplicitEulerPhysicsSystem);

        public void Init()
        {
        }

        public void Update(TimingInfo timingInfo)
        {
            var deltaTime = (float)timingInfo.ElapsedTime.TotalSeconds;
            _gameObjectManager.GameObjects
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
            var currentVelocity = physicsComponent.Velocity;
            var previousAcceleration = physicsComponent.PreviousAcceleration;

            var newVelocity = currentVelocity + (deltaTime * previousAcceleration);
            var newPosition = currentPosition + (deltaTime * newVelocity);

            transform.Position = newPosition;
            physicsComponent.Velocity = newVelocity;
            physicsComponent.Acceleration = newAcceleration;

            physicsComponent.ImpulseForce = Vector2.Zero;
        }

        private Vector2 ApplyForces(PhysicsComponent physicsComponent)
        {
            return (physicsComponent.Gravity + physicsComponent.ImpulseForce) * physicsComponent.MassInverted;
        }
    }
}
