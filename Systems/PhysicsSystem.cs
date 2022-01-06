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
using Windows.Foundation;
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
            var deltaTime = (float)timingInfo.ElapsedTime.TotalSeconds;

            _gameObjectManager.GameObjects
                .Where(gameObject => _entitiesInContact.ContainsKey(gameObject.Id) && gameObject.Has<PhysicsComponent, TransformComponent, ColliderComponent>())
                .Select(gameObject => (entityId: gameObject.Id, gameObject.GetComponents<PhysicsComponent, TransformComponent, ColliderComponent>()))
                .ToArray() // clone
                .ForEach(result =>
                {
                    var physicsComponent = result.Item2.Item1;
                    var transformComponent = result.Item2.Item2;
                    var colliderComponent = result.Item2.Item3;
                    var halfSize = colliderComponent.BoundingBox.Half();

                    // if there is a collision, we should apply a force such that sum of forces avoids the collision
                    // or responds in ways
                    foreach (var collisionManifold in _entitiesInContact[result.entityId].OrderBy(collisionManifold => collisionManifold.CollisionTime))
                    {
                        if (!CollisionDetection.IsRectInRect(colliderComponent.BoundingBox,
                              physicsComponent.Position,
                              collisionManifold.ContactRect,
                              out var contactPoint,
                              out var contactNormal,
                              out var contactTime))
                        {
                            continue;
                        }

                        // Push out of collision
                        if (contactNormal.X != 0f)
                        {
                            physicsComponent.Position = new Vector2(contactPoint.X - halfSize.X, physicsComponent.Position.Y);
                        }

                        if (contactNormal.Y != 0)
                        {
                            physicsComponent.Position = new Vector2(physicsComponent.Position.X, contactPoint.Y - halfSize.Y);
                        }

                        // Calculate new position that moves the object out of collision
                        if (contactNormal.Y <= 0 && contactNormal.Y >= 0 && contactNormal.X <= 0 && contactNormal.X >= 0)
                        {
                            System.Diagnostics.Debug.WriteLine("No normal! {0}", collisionManifold.CollisionPoint);
                            continue;
                        }

                        var remainingTime = 1f - contactTime;
                        var velocity = physicsComponent.Velocity;
                        var dotProductVelocity = Vector2.Dot(velocity, contactNormal) * remainingTime;
                        var velocityChange = Math.Abs(dotProductVelocity) * contactNormal;
                        physicsComponent.Velocity += velocityChange;

                        // Sliding response
                        //var currentVelocity = (physicsComponent.Position - transformComponent.Position) / deltaTime;
                        //var previousVelocity = physicsComponent.PreviousVelocity;
                        //var currentAcceleration = physicsComponent.PreviousAcceleration;
                        //var remainingTime = 1f - collisionManifold.CollisionTime;
                        //var dotProductVelocity = Vector2.Dot(currentVelocity, collisionManifold.CollisionNormal) * remainingTime;
                        //var dotProductAcceleration = Vector2.Dot(currentAcceleration, collisionManifold.CollisionNormal) * remainingTime;
                        //var velocityChange = Math.Abs(dotProductVelocity) * collisionManifold.CollisionNormal;
                        //var accelerationChange = Math.Abs(dotProductAcceleration) * collisionManifold.CollisionNormal;

                        //physicsComponent.ImpulseForce += accelerationChange * physicsComponent.Mass;
                        //var resultingVelocity = currentVelocity + velocityChange;
                        //var resultingAcceleration = currentAcceleration - accelerationChange;
                        //var newPosition = transformComponent.Position + (float)timingInfo.ElapsedTime.TotalSeconds * resultingVelocity;
                        //var newPosition = CalculatePosition(remainingTime, transformComponent.Position, resultingVelocity, resultingAcceleration);
                        //physicsComponent.Velocity = resultingVelocity;// dotProduct * collisionManifold.CollisionNormal;// new Vector2(dotProduct * collisionManifold.CollisionNormal.Y, dotProduct * collisionManifold.CollisionNormal.X);
                        //physicsComponent.Acceleration = resultingAcceleration;
                        //physicsComponent.Position = newPosition;
                    }
                });

            //_gameObjectManager.GameObjects
            //    .Where(gameObject => _entitiesInContact.ContainsKey(gameObject.Id))
            //    .Select(gameObject => (gameObject, gameObject.GetComponents<PhysicsComponent, ColliderComponent, TransformComponent>(), collisionManifolds: _entitiesInContact[gameObject.Id]))
            //    .Where(result => result != default && result.Item2 != default)
            //    .ToArray() // clone
            //    .ForEach(result =>
            //    {
            //        var physicsComponent = result.Item2.Item1;
            //        var colliderComponent = result.Item2.Item2;
            //        var transformComponent = result.Item2.Item3;

            //        var position = physicsComponent.Position;
            //        var halfSize = colliderComponent.BoundingBox.Half();

            //        foreach (var collisionManifold in result.collisionManifolds.OrderBy(collisionManifold => collisionManifold.CollisionTime))
            //        {
            //            var contactNormal = collisionManifold.CollisionNormal;
            //            var contactPoint = collisionManifold.CollisionPoint;

            //            // Calculate new position that moves the object out of collision
            //            if (contactNormal.Y > 0 || contactNormal.Y < 0)
            //            {
            //                position.Y = collisionManifold.CollisionPoint.Y - halfSize.Y;
            //            }
            //            else if (contactNormal.X > 0 || contactNormal.X < 0)
            //            {
            //                position.X = collisionManifold.CollisionPoint.X - halfSize.X;
            //            }
            //            else
            //            {
            //                System.Diagnostics.Debug.WriteLine("No normal! {0}", collisionManifold.CollisionPoint);
            //                //position = collisionManifold.CollisionPoint;
            //                continue;
            //            }

            //            //position = position + contactNormal * (contactPoint - halfSize);

            //            //physicsComponent.Position = position;

            //            // Sliding response
            //            //var deltaTime = (float)timingInfo.ElapsedTime.TotalSeconds;
            //            var currentVelocity = (physicsComponent.Position - transformComponent.Position) / deltaTime;
            //            var previousVelocity = physicsComponent.PreviousVelocity;
            //            var currentAcceleration = physicsComponent.PreviousAcceleration;
            //            var remainingTime = 1f - collisionManifold.CollisionTime;
            //            var dotProductVelocity = Vector2.Dot(currentVelocity, collisionManifold.CollisionNormal) * remainingTime;
            //            var dotProductAcceleration = Vector2.Dot(currentAcceleration, collisionManifold.CollisionNormal) * remainingTime;
            //            var velocityChange = dotProductVelocity * collisionManifold.CollisionNormal;
            //            var accelerationChange = dotProductAcceleration * collisionManifold.CollisionNormal;
            //            var resultingVelocity = currentVelocity - velocityChange;
            //            var resultingAcceleration = currentAcceleration - accelerationChange;
            //            var newPosition = transformComponent.Position + (float)timingInfo.ElapsedTime.TotalSeconds * resultingVelocity;
            //            //var newPosition = CalculatePosition(remainingTime, transformComponent.Position, resultingVelocity, resultingAcceleration);
            //            physicsComponent.Velocity = resultingVelocity;// dotProduct * collisionManifold.CollisionNormal;// new Vector2(dotProduct * collisionManifold.CollisionNormal.Y, dotProduct * collisionManifold.CollisionNormal.X);
            //            physicsComponent.Acceleration = resultingAcceleration;
            //            physicsComponent.Position = newPosition;
            //            break;

            //            // Impulse based response
            //            //var j = -(1f + physicsComponent.RestitutionFactor) * Vector2.Dot(physicsComponent.Velocity, collisionManifold.CollisionNormal);
            //            //j *= physicsComponent.MassInverted;
            //            //var newVelocity = physicsComponent.Velocity + (j / physicsComponent.Mass * collisionManifold.CollisionNormal);
            //            //physicsComponent.Velocity = newVelocity;
            //        }
            //});

            _entitiesInContact.Clear();
        }

        private Vector2 GetResponseVelocity(PhysicsComponent physicsComponent, Vector2 normal, float coefficient)
        {
            var linearMomentum = GetLinearCollisionImpulse(physicsComponent, normal, coefficient);

            return linearMomentum * physicsComponent.MassInverted;
        }

        private Vector2 GetLinearCollisionImpulse(PhysicsComponent physicsComponent, Vector2 normal, float coefficient)
        {
            var linearMomentum = physicsComponent.Acceleration;//.Velocity * physicsComponent.Mass;
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
