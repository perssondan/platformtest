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
using uwpPlatformer.Numerics;
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
        /// <param name="deltaTime"></param>
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
                if (!IsRectInRect(dynamicCollider.BoundingBox,
                                  dynamicCollider.GameObject.PhysicsComponent.Position,
                                  componentInCollision.BoundingBox,
                                  dynamicCollider,
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
            foreach (var collider in collidersToTest)
            {
                if (IsAABBColliding(dynamicRect, collider.BoundingBox))
                    yield return collider;
            }
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

        private bool IsRayInRect(LineSegment ray,
                                 Rect staticRect,
                                 out Vector2 contactPoint,
                                 out Vector2 contactNormal,
                                 out float contactTime)
        {
            contactPoint = Vector2.Zero;
            contactNormal = Vector2.Zero;
            contactTime = 0f;

            var minPos = staticRect.TopLeft();
            var maxPos = minPos + staticRect.Size();

            // Calculate intersection with rectangle bounding axes
            var minContactPoint = (minPos - ray.StartPoint) * ray.InvDirection;
            var maxContactPoint = (maxPos - ray.StartPoint) * ray.InvDirection;

            if (float.IsNaN(maxContactPoint.X) || float.IsNaN(maxContactPoint.Y)) return false;
            if (float.IsNaN(minContactPoint.X) || float.IsNaN(minContactPoint.Y)) return false;

            // Swap
            if (minContactPoint.X > maxContactPoint.X) ObjectExtensions.Swap(ref minContactPoint.X, ref maxContactPoint.X);

            // Swap
            if (minContactPoint.Y > maxContactPoint.Y) ObjectExtensions.Swap(ref minContactPoint.Y, ref maxContactPoint.Y);

            if (minContactPoint.X > maxContactPoint.Y || minContactPoint.Y > maxContactPoint.X) return false;

            // Min 'time' will be the first contact
            contactTime = Math.Max(minContactPoint.X, minContactPoint.Y);

            // Max 'time' will contact on the opposite side of the target
            var maxIntersectionLength = Math.Min(maxContactPoint.X, maxContactPoint.Y);

            // if 'time' is less than zero, the segment started inside the box,
            // we'd like to set the contact point on the oposite direction of the ray
            // to push it out of the presumed direction it had when it entered.
            if (contactTime < 0f)
            {
                contactTime = maxIntersectionLength;
            }

            // If negative it is pointing away from target
            if (maxIntersectionLength < 0) return false;
            
            // Contact point of collision from parametric line equation
            contactPoint = ray.StartPoint + contactTime * ray.Direction;

            if (minContactPoint.X > minContactPoint.Y)
            {
                if (ray.InvDirection.X < 0f)
                {
                    contactNormal = new Vector2(1, 0);
                }
                else
                {
                    contactNormal = new Vector2(-1, 0);
                }
            }
            else if (minContactPoint.X < minContactPoint.Y)
            {
                if (ray.InvDirection.Y < 0f)
                {
                    contactNormal = new Vector2(0, 1);
                }
                else
                {
                    contactNormal = new Vector2(0, -1);
                }
            }

            return true;
        }

        private bool IsRectInRect(Rect dynamicRect,
                                  Vector2 newPosition,
                                  Rect staticRect,
                                  ColliderComponent colliderComponent,
                                  out Vector2 contactPoint,
                                  out Vector2 contactNormal,
                                  out float contactTime)
        {
            var originPosition = dynamicRect.TopLeft();
            var centerPoint = dynamicRect.Center();
            var centerOffset = centerPoint - originPosition;
            var newCenterPoint = newPosition + centerOffset;

            var expandedStaticRect = staticRect.Add(dynamicRect.Size());

            var lineSegment = new LineSegment(centerPoint, newCenterPoint);

            if (centerPoint == newCenterPoint)
            {
                System.Diagnostics.Debug.WriteLine("No movement!");
            }

            var isRayInRect = IsRayInRect(lineSegment,
                            expandedStaticRect,
                            out contactPoint,
                            out contactNormal,
                            out contactTime);

            return isRayInRect && contactTime >= 0f && contactTime < 1f;
        }

        private bool IsAABBColliding(Rect sourceRect, Rect opponentRect)
        {
            return (sourceRect.Left < opponentRect.Right && sourceRect.Right > opponentRect.Left) &&
                    (sourceRect.Top < opponentRect.Bottom && sourceRect.Bottom > opponentRect.Top);
        }

        private bool AABBvsAABB(Rect first, Rect other)
        {
            if (first.Left > other.Right)
                return false;

            if (first.Right < other.Left)
                return false;

            if (first.Bottom > other.Top)
                return false;

            if (first.Top < other.Bottom)
                return false;

            return true;
        }

        public Rect GetMinkowskiDifference(Rect first, Rect other)
        {
            var topLeft = first.TopLeft() - other.TopRight();
            var fullSize = first.Size() + other.Size();
            var halfFullSize = fullSize / 2;
            return new Rect((topLeft + halfFullSize).ToPoint(), halfFullSize.ToSize());
        }

        public bool intersectAABB(Rect dynamicRect, Rect staticRect, out Vector2 normal, out Vector2 contactPoint)
        {
            normal = Vector2.Zero;
            contactPoint = Vector2.Zero;
            var dx = dynamicRect.Left - staticRect.Left;
            var px = dynamicRect.Half().X + staticRect.Half().X - Math.Abs(dx);
            if (px <= 0) return false;

            var dy = dynamicRect.Top - staticRect.Top;
            var py = dynamicRect.Half().Y + staticRect.Half().Y - Math.Abs(dy);
            if (py <= 0) return false;

            if (px < py)
            {
                var sx = Math.Sign(dx);
                normal.X = sx;
                contactPoint.X = (float)staticRect.Left + (staticRect.Half().X * sx);
                contactPoint.Y = (float)dynamicRect.Center().Y;
            }
            else
            {
                var sy = Math.Sign(dy);
                normal.Y = sy;
                contactPoint.X = (float)dynamicRect.Center().X;
                contactPoint.Y = (float)staticRect.Top + (staticRect.Half().Y * sy);
            }

            return true;
        }
    }
}
