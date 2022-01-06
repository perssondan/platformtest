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
using Windows.Foundation;

namespace uwpPlatformer.Systems
{
    public class ColliderSystem : ISystem
    {
        private readonly IEventSystem _eventSystem;
        private readonly IGameObjectManager _gameObjectManager;

        public ColliderSystem(IEventSystem eventSystem, IGameObjectManager gameObjectManager)
        {
            _eventSystem = eventSystem;
            _gameObjectManager = gameObjectManager;
        }

        public string Name => nameof(ColliderSystem);

        public void Update(TimingInfo timingInfo)
        {
            var collidersToTest = GetAllColliderComponents();
            ResetCollisionFlag(collidersToTest);
            DetectCollisions(collidersToTest);
        }

        private ColliderComponent[] GetAllColliderComponents()
        {
            return _gameObjectManager.GameObjects
                .Where(gameObject => gameObject.Has<ColliderComponent>())
                .Select(gameObject => gameObject.GetComponent<ColliderComponent>())
                .ToArray();
        }

        /// <summary>
        /// Finds and updates collision infos
        /// </summary>
        /// <param name="dynamicCollider"></param>
        /// <param name="colliderComponents"></param>
        /// <returns>Returns true if there is collisions, otherwise false.</returns>
        private bool DetectAndUpdateCollisionInfos(ColliderComponent dynamicCollider, ColliderComponent[] colliderComponents)
        {
            dynamicCollider.CollisionManifolds = Array.Empty<CollisionManifold>();

            if ((dynamicCollider.CollisionType & ColliderComponent.CollisionTypes.IsDynamicMask) == 0) return false;

            // Broad phase collision detection
            var componentsInCollision = GetOverlappingColliders(dynamicCollider, colliderComponents);

            // Detail phase collision detection
            var results = new List<CollisionManifold>();
            foreach (var componentInCollision in componentsInCollision)
            {
                if (!CollisionDetection.IsRectInRect(dynamicCollider.BoundingBox,
                                  dynamicCollider.GameObject.PhysicsComponent.Position,
                                  componentInCollision.BoundingBox,
                                  out var contactPoint,
                                  out var contactNormal,
                                  out var contactTime))
                    continue;

                var collisionInfo = new CollisionManifold(contactPoint, contactNormal, contactTime, componentInCollision.BoundingBox);

                results.Add(collisionInfo);
                componentInCollision.IsColliding = true;
                _eventSystem.Send(this, new CollisionEvent(dynamicCollider.GameObject, componentInCollision.GameObject, collisionInfo));
            }

            if (results.Any())
            {
                dynamicCollider.CollisionManifolds = results.OrderBy(info => info.CollisionTime).ToArray();
                dynamicCollider.IsColliding = true;
                return true;
            }

            return false;
        }

        private void ResetCollisionFlag(ColliderComponent[] colliderComponents)
        {
            colliderComponents
                .ForEach(collider => collider.IsColliding = false);
        }

        private bool DetectCollisions(ColliderComponent[] colliderComponents)
        {
            var dynamicColliders = colliderComponents
                .Where(collider => (collider.CollisionType & ColliderComponent.CollisionTypes.IsDynamicMask) > 0)
                .ToArray();

            if (!dynamicColliders.Any()) return false;

            var isAnyColliding = false;
            dynamicColliders.ForEach(dynamicCollider =>
            {
                var isColliding = DetectAndUpdateCollisionInfos(dynamicCollider, colliderComponents);
                isAnyColliding |= isColliding;
            });

            return isAnyColliding;
        }

        private IEnumerable<ColliderComponent> GetOverlappingColliders(Rect dynamicRect, ColliderComponent[] collidersToTest)
        {
            return collidersToTest.Where(collider => CollisionDetection.IsAABBColliding(dynamicRect, collider.BoundingBox));
        }

        private ColliderComponent[] GetOverlappingColliders(ColliderComponent dynamicCollider, ColliderComponent[] colliderComponents)
        {
            var sweptBroadPhaseRect = GetSweptBroadphaseRect(dynamicCollider);
            dynamicCollider.MovingBoundingBox = sweptBroadPhaseRect;
            return GetOverlappingColliders(sweptBroadPhaseRect, colliderComponents.Except(new[] { dynamicCollider }).ToArray())
                .ToArray();
        }

        /// <summary>
        /// Calculates the bounding box from current position to future position
        /// </summary>
        /// <param name="dynamicCollider"></param>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        private static Rect GetSweptBroadphaseRect(ColliderComponent dynamicCollider)
        {
            var physicsComponent = dynamicCollider.GameObject.PhysicsComponent;
            var futurePosition = physicsComponent.Position;
            var unionDynamicRect = new Rect(futurePosition.X, futurePosition.Y, dynamicCollider.BoundingBox.Width, dynamicCollider.BoundingBox.Height);
            unionDynamicRect.Union(dynamicCollider.BoundingBox);
            return unionDynamicRect;
        }

        public Rect GetMinkowskiDifference(Rect first, Rect other)
        {
            var topLeft = first.TopLeft() - other.TopRight();
            var fullSize = first.Size() + other.Size();
            var halfFullSize = fullSize / 2;
            return new Rect((topLeft + halfFullSize).ToPoint(), halfFullSize.ToSize());
        }
    }
}
