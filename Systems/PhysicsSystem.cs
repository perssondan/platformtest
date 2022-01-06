using GamesLibrary.Models;
using GamesLibrary.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using uwpPlatformer.Components;
using uwpPlatformer.Events;
using uwpPlatformer.Extensions;
using uwpPlatformer.GameObjects;
using uwpPlatformer.Utilities;

namespace uwpPlatformer.Systems
{
    public class PhysicsSystem : ISystem
    {
        private readonly IGameObjectManager _gameObjectManager;
        private readonly IEventSystem _eventSystem;
        private readonly Dictionary<int, IList<CollisionManifold>> _entitiesInContact = new Dictionary<int, IList<CollisionManifold>>();

        public PhysicsSystem(IGameObjectManager gameObjectManager, IEventSystem eventSystem)
        {
            _gameObjectManager = gameObjectManager;
            _eventSystem = eventSystem;

            _eventSystem.Subscribe<CollisionEvent>(this, (sender, collisionEvent) =>
            {
                // We're not interested in collisions between two physics components, since we can't yet handle this types of collisions
                if (collisionEvent.GameObject.Has<PhysicsComponent>() && collisionEvent.IsCollidingWith.Has<PhysicsComponent>()) return;

                var collisionTime = collisionEvent.CollisionManifold.CollisionTime;

                // if not yet in our list, add it
                if (!_entitiesInContact.ContainsKey(collisionEvent.GameObject.Id))
                {
                    _entitiesInContact.Add(collisionEvent.GameObject.Id, new List<CollisionManifold>(new[] { collisionEvent.CollisionManifold }));
                    return;
                }

                _entitiesInContact[collisionEvent.GameObject.Id].Add(collisionEvent.CollisionManifold);
            });
        }

        public string Name => nameof(PhysicsSystem);

        public void Update(TimingInfo timingInfo)
        {
            _entitiesInContact.Clear();

            var deltaTime = (float)timingInfo.ElapsedTime.TotalSeconds;

            _gameObjectManager.GameObjects
                .Where(gameObject => gameObject.Has<PhysicsComponent>())
                .Select(gameObject => gameObject.GetComponents<PhysicsComponent, TransformComponent>())
                .ToArray() // clone
                .ForEach(result =>
                {
                    Integrate(result.Item1, result.Item2, deltaTime);
                });
        }

        public void PostUpdate(TimingInfo timingInfo)
        {
            _gameObjectManager.GameObjects
                .Where(gameObject => _entitiesInContact.ContainsKey(gameObject.Id) && gameObject.Has<PhysicsComponent, ColliderComponent>())
                .Select(gameObject => (entityId: gameObject.Id, gameObject.GetComponents<PhysicsComponent, ColliderComponent>()))
                .ToArray() // clone
                .ForEach(result =>
                {
                    var physicsComponent = result.Item2.Item1;
                    var colliderComponent = result.Item2.Item2;
                    HandleCollision(physicsComponent, colliderComponent, _entitiesInContact[result.entityId].OrderBy(collisionManifold => collisionManifold.CollisionTime));
                });
            _entitiesInContact.Clear();
        }

        private void HandleCollision(PhysicsComponent physicsComponent, ColliderComponent colliderComponent, IEnumerable<CollisionManifold> collisionManifolds)
        {
            var halfSize = colliderComponent.BoundingBox.Half();
            foreach (var collisionManifold in collisionManifolds)
            {
                // What if multiple collisions are still going on after resolving the first one, but the contaxt time is not in the right order?
                if (!CollisionDetection.IsRectInRect(colliderComponent.BoundingBox,
                      physicsComponent.Position,
                      collisionManifold.ContactRect,
                      out var contactPoint,
                      out var contactNormal,
                      out var contactTime))
                {
                    continue;
                }

                ResolvePenetration(physicsComponent, contactNormal, contactPoint, halfSize);

                // No normal
                if (contactNormal == Vector2.Zero)
                {
                    System.Diagnostics.Debug.WriteLine("No normal! {0} -> {1}", contactPoint, physicsComponent.Position);
                    continue;
                }

                var remainingTime = 1f - contactTime;
                SlideResponse(physicsComponent, contactNormal, remainingTime);
            }
        }

        private void ResolvePenetration(PhysicsComponent physicsComponent, Vector2 contactNormal, Vector2 contactPoint, Vector2 halfSize)
        {
            // Push entity out of collision

            if (contactNormal.X != 0f)
            {
                physicsComponent.Position = new Vector2(contactPoint.X - halfSize.X, physicsComponent.Position.Y);
            }

            if (contactNormal.Y != 0)
            {
                physicsComponent.Position = new Vector2(physicsComponent.Position.X, contactPoint.Y - halfSize.Y);
            }
        }

        private void SlideResponse(PhysicsComponent physicsComponent, Vector2 contactNormal, float remainingTime)
        {
            var velocity = physicsComponent.Velocity;
            var dotProductVelocity = Vector2.Dot(velocity, contactNormal) * remainingTime;
            var velocityChange = Math.Abs(dotProductVelocity) * contactNormal;
            physicsComponent.Velocity += velocityChange;
        }

        private void ImpulseResponse(PhysicsComponent physicsComponent, Vector2 contactNormal)
        {
            // Where is remaining time in this one?
            var j = -(1f + physicsComponent.RestitutionFactor) * Vector2.Dot(physicsComponent.Velocity, contactNormal);
            j *= physicsComponent.MassInverted;
            var newVelocity = physicsComponent.Velocity + (j / physicsComponent.Mass * contactNormal);
            physicsComponent.Velocity = newVelocity;
        }

        private Vector2 GetResponseVelocity(PhysicsComponent physicsComponent, Vector2 normal, float coefficient)
        {
            var linearMomentum = GetLinearCollisionImpulse(physicsComponent, normal, coefficient);

            return linearMomentum * physicsComponent.MassInverted;
        }

        private Vector2 GetLinearCollisionImpulse(PhysicsComponent physicsComponent, Vector2 normal, float coefficient)
        {
            var linearMomentum = physicsComponent.Acceleration;
            var dot = Vector2.Dot(linearMomentum, normal);
            var j = MathF.Max(-(1f + coefficient) * dot, 0);
            var newLinearMomentum = j * normal;
            return newLinearMomentum;
        }

        private Vector2 ApplyForces(PhysicsComponent physicsComponent)
        {
            var dragForce = 0.5f * physicsComponent.Drag * (physicsComponent.Velocity * Vector2.Abs(physicsComponent.Velocity));
            // acc = F/m (F*1/m)
            return physicsComponent.Gravity + ((physicsComponent.ImpulseForce - dragForce) * physicsComponent.MassInverted);
        }

        private void Integrate(PhysicsComponent physicsComponent, TransformComponent transformComponent, float deltaTime)
        {
            var currentPosition = transformComponent.Position;
            var currentVelocity = physicsComponent.Velocity;
            var currentAcceleration = physicsComponent.Acceleration;

            var newPosition = CalculatePosition(deltaTime, currentPosition, currentVelocity, currentAcceleration);
            var newAcceleration = ApplyForces(physicsComponent);
            var newVelocity = CalculateVelocity(deltaTime, currentVelocity, currentAcceleration, newAcceleration);

            physicsComponent.Position = newPosition;
            physicsComponent.Velocity = newVelocity;
            physicsComponent.Acceleration = newAcceleration;

            // Reset any impulse forces, they've been integrated.
            physicsComponent.ImpulseForce = Vector2.Zero;
        }

        private static Vector2 CalculatePosition(float deltaTime, Vector2 currentPosition, Vector2 currentVelocity, Vector2 currentAcceleration)
        {
            return currentPosition + (currentVelocity * deltaTime) + (currentAcceleration * deltaTime * deltaTime * 0.5f);
        }

        private static Vector2 CalculateVelocity(float deltaTime, Vector2 currentVelocity, Vector2 currentAcceleration, Vector2 newAcceleration)
        {
            var newVelocity = currentVelocity + ((newAcceleration + currentAcceleration) * deltaTime * 0.5f);

            if (newVelocity == Vector2.Zero) return newVelocity;

            if (Math.Abs(newVelocity.X) < 1f)
                newVelocity.X = 0f;

            if (Math.Abs(newVelocity.Y) < 1f)
                newVelocity.Y = 0f;

            return newVelocity;
        }
    }
}
