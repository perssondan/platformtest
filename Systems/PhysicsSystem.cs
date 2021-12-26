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
                .Select(gameObject => gameObject.GetComponent<PhysicsComponent>())
                .ToArray() // clone
                .ForEach(physicsComponent =>
                {
                    Integrate(physicsComponent, deltaTime);
                });
        }

        private HashSet<(float, float, float)> _history = new HashSet<(float, float, float)>();

        public void PostUpdate(TimingInfo timingInfo)
        {
            _gameObjectManager.GameObjects
                .Where(gameObject => _entitiesInContact.ContainsKey(gameObject.Id))
                .Select(gameObject => (gameObject, gameObject.GetComponents<PhysicsComponent, ColliderComponent>(), collisionManifolds: _entitiesInContact[gameObject.Id]))
                .Where(result => result != default && result.Item2 != default)
                .ToArray() // clone
                .ForEach(result =>
                {
                    var physicsComponent = result.Item2.Item1;
                    var colliderComponent = result.Item2.Item2;

                    var position = physicsComponent.Position;
                    var halfSize = colliderComponent.BoundingBox.Half();

                    foreach (var collisionManifold in result.collisionManifolds.OrderBy(collisionManifold => collisionManifold.CollisionTime))
                    {
                        var contactNormal = collisionManifold.CollisionNormal;
                        var contactPoint = collisionManifold.CollisionPoint;

                        // Calculate new position that moves the object out of collision
                        if (contactNormal.Y > 0 || contactNormal.Y < 0)
                        {
                            position.Y = collisionManifold.CollisionPoint.Y - colliderComponent.BoundingBox.Half().Y;
                        }
                        else if (contactNormal.X > 0 || contactNormal.X < 0)
                        {
                            position.X = collisionManifold.CollisionPoint.X - colliderComponent.BoundingBox.Half().X;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("No normal!");
                            position = collisionManifold.CollisionPoint;
                        }

                        physicsComponent.Position = position;
                        //var impulseForce = GetLinearCollisionImpulse(physicsComponent, contactNormal, 0f);
                        //physicsComponent.ImpulseForce += impulseForce;
                        //physicsComponent.Velocity = physicsComponent.Velocity + (impulseForce * physicsComponent.MassInverted);
                    }
                });

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

        //void ApplyLinearCollisionImpulse(StaticContact & contact, float e)
        //{
        //    //float mass = contact.rigidBody->mass;
        //    //float d = dot(contact.rigidBody->linearMomentum, contact.normal);
        //    //float j = max(-(1 + e) * d, 0);
        //    //contact.rigidBody->linearMomentum += j * contact.normal;
        //}

        private Vector2 ApplyForces(PhysicsComponent physicsComponent)
        {
            var dragForce = 0.5f * physicsComponent.Drag * (physicsComponent.Velocity * Vector2.Abs(physicsComponent.Velocity));
            // acc = F/m (F*1/m)
            return (physicsComponent.GravityForce + physicsComponent.ImpulseForce - dragForce) * physicsComponent.MassInverted;
        }

        private void Integrate(PhysicsComponent physicsComponent, float deltaTime)
        {
            var currentPosition = physicsComponent.Position;
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
