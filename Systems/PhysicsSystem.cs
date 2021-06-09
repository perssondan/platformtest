using GamesLibrary.Models;
using GamesLibrary.Systems;
using System;
using System.Linq;
using System.Numerics;
using uwpPlatformer.Components;
using uwpPlatformer.Extensions;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Systems
{
    public class PhysicsSystem : SystemBase<PhysicsSystem>
    {
        private readonly IGameObjectManager _gameObjectManager;

        public PhysicsSystem(IGameObjectManager gameObjectManager)
        {
            _gameObjectManager = gameObjectManager;
        }

        public override void Update(TimingInfo timingInfo)
        {
            var deltaTime = (float)timingInfo.ElapsedTime.TotalSeconds;

            _gameObjectManager.GameObjects
                .Select(gameObject => (gameObject, gameObject.GetComponents<PhysicsComponent, TransformComponent>()))
                .Where(result => result != default && result.Item2 != default)
                .ToArray() // clone
                .ForEach(result =>
                {
                    Integrate(result.Item2.Item1, result.Item2.Item2, deltaTime);
                });
        }

        private Vector2 ApplyForces(PhysicsComponent physicsComponent)
        {
            // acc = F/m (F*1/m)
            return (physicsComponent.Gravity + physicsComponent.ImpulseForce) * physicsComponent.MassInverted;
        }

        private void Integrate(PhysicsComponent physicsComponent, TransformComponent transformComponent, float deltaTime)
        {
            var newAcceleration = ApplyForces(physicsComponent);
            var newVelocity = transformComponent.Velocity + ((physicsComponent.OldAcceleration + newAcceleration) * (deltaTime * 0.5f));

            physicsComponent.OldAcceleration = newAcceleration;
            transformComponent.Velocity = newVelocity;
            physicsComponent.ImpulseForce = Vector2.Zero;
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
