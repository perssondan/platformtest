﻿using GamesLibrary.Models;
using System;
using System.Linq;
using System.Numerics;
using uwpPlatformer.Components;
using uwpPlatformer.Extensions;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Systems
{
    /// <summary>
    /// For reference, Improved Euler integration
    /// </summary>
    public class ImprovedEulerPhysicsSystem : ISystem
    {
        private readonly IGameObjectManager _gameObjectManager;

        public ImprovedEulerPhysicsSystem(IGameObjectManager gameObjectManager)
        {
            _gameObjectManager = gameObjectManager;
        }

        public string Name => nameof(ImprovedEulerPhysicsSystem);

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
            var deltaTime = (float)timeSpan.TotalSeconds;

            var currentVelocity = physicsComponent.Velocity;
            var previousAcceleration = physicsComponent.PreviousAcceleration;

            var newAcceleration = ApplyForces(physicsComponent);
            var newVelocity = currentVelocity + deltaTime * previousAcceleration;
            var newPosition = (currentVelocity + newVelocity) * 0.5f * deltaTime;

            physicsComponent.Velocity = newVelocity;
            transformComponent.Position = newPosition;
            physicsComponent.Acceleration = newAcceleration;

            physicsComponent.ImpulseForce = Vector2.Zero;
        }

        private Vector2 ApplyForces(PhysicsComponent physicsComponent)
        {
            return (physicsComponent.Gravity + physicsComponent.ImpulseForce) * physicsComponent.MassInverted;
        }
    }
}
