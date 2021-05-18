﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using uwpKarate.Components;
using uwpKarate.Events;
using uwpKarate.Extensions;
using uwpKarate.GameObjects;
using uwpKarate.Numerics;
using Windows.Foundation;

namespace uwpKarate.Systems
{
    public class ColliderSystem : SystemBase<ColliderSystem>
    {
        public override void Update(TimeSpan deltaTime)
        {
            DetectCollisions(deltaTime);
        }

        public void ResolveCollisions(TimeSpan deltaTime)
        {
            var fDeltaTime = (float)deltaTime.TotalSeconds;
            var dynamicColliderComponents = ColliderComponentManager.Instance.Components
                .Where(colliderComponent => colliderComponent.IsColliding == true)
                .Where(colliderComponent => colliderComponent.CollisionType == ColliderComponent.CollisionTypes.Dynamic)
                .ToArray();

            foreach (var colliderComponent in dynamicColliderComponents)
            {
                var dynamicColliderWasMoved = TryResolveCollisions(colliderComponent, fDeltaTime)
                    .Any(value => value == true);
                if (dynamicColliderWasMoved)
                {
                    DetectCollisions(deltaTime);
                    ResolveCollisions(deltaTime);
                    return;
                }
            }
        }

        public bool TryResolveCollision(ColliderComponent colliderComponent, CollisionInfo collisionInfo, float deltaTime)
        {
            var transformComponent = colliderComponent.GameObject.TransformComponent;
            if (!IsRectInRect(colliderComponent.BoundingBox,
                              transformComponent.Velocity,
                              collisionInfo.ContactRect,
                              out var contactPoint,
                              out var contactNormal,
                              out var contactTime,
                              deltaTime))
                return false;

            // Nothing to resolve
            if (contactNormal == Vector2.Zero)
                return false;

            transformComponent.Velocity += contactNormal * new Vector2(Math.Abs(transformComponent.Velocity.X), Math.Abs(transformComponent.Velocity.Y)) * (1f - contactTime);
            return true;
        }

        public bool IsColliding(Vector2 futurePosition, ColliderComponent colliderComponent)
        {
            var collidersToTest = ColliderComponentManager.Instance.Components.Where(collider => collider != colliderComponent);
            var futureRect = colliderComponent.BoundingBox;
            futureRect.X = futurePosition.X;
            futureRect.Y = futurePosition.Y;

            return collidersToTest.Any(collider => IsColliding(futureRect, collider.BoundingBox));
        }

        public bool IsColliding(Rect sourceRect, Rect opponentRect)
        {
            return (sourceRect.Left < opponentRect.Right && sourceRect.Right > opponentRect.Left) &&
                    (sourceRect.Top < opponentRect.Bottom && sourceRect.Bottom > opponentRect.Top);
        }

        public bool IsColliding(ColliderComponent first, ColliderComponent second)
        {
            return IsColliding(first.BoundingBox, second.BoundingBox);
        }

        public bool IsColliding(ColliderComponent dynamicCollider, float deltaTime)
        {
            dynamicCollider.CollisionInfos = Array.Empty<CollisionInfo>();

            if (dynamicCollider.CollisionType == ColliderComponent.CollisionTypes.Static) return false;

            var colliderComponents = ColliderComponentManager.Instance.Components
                .Where(collider => collider.CollisionType == ColliderComponent.CollisionTypes.Static)
                .ToArray();

            // reset collision flag
            colliderComponents
                .ForEach(collider => collider.IsColliding = false);

            var componentsInCollision = GetOverlappingColliders(dynamicCollider, deltaTime, colliderComponents);

            var results = new List<CollisionInfo>();
            foreach (var componentInCollision in componentsInCollision)
            {
                if (!IsRectInRect(dynamicCollider.BoundingBox, dynamicCollider.GameObject.TransformComponent.Velocity, componentInCollision.BoundingBox, out var contactPoint, out var contactNormal, out var contactTime, deltaTime))
                    continue;

                var collisionInfo = new CollisionInfo(contactPoint, contactNormal, contactTime)
                {
                    ContactRect = componentInCollision.BoundingBox
                };
                results.Add(collisionInfo);
                componentInCollision.IsColliding = true;
                EventSystem.Instance.Send(new CollisionArgument { GameObject = dynamicCollider.GameObject, IsCollidingWith = componentInCollision.GameObject, CollisionInfo = collisionInfo });
            }

            if (results.Any())
            {
                dynamicCollider.CollisionInfos = results.OrderBy(info => info.CollisionTime).ToArray();
                return true;
            }

            return false;
        }

        private bool DetectCollisions(TimeSpan deltaTime)
        {
            var dynamicColliders = ColliderComponentManager.Instance.Components
                            .Where(collider => collider.CollisionType == ColliderComponent.CollisionTypes.Dynamic);
            if (!dynamicColliders.Any()) return false;

            var isAnyColliding = false;
            dynamicColliders.ForEach(dynamicCollider =>
            {
                dynamicCollider.IsColliding = false;

                var isColliding = IsColliding(dynamicCollider, (float)deltaTime.TotalSeconds);
                isAnyColliding |= isColliding;

                dynamicCollider.IsColliding = isColliding;
            });

            return isAnyColliding;
        }

        private IEnumerable<bool> TryResolveCollisions(ColliderComponent colliderComponent, float fDeltaTime)
        {
            foreach (var collisionInfo in colliderComponent.CollisionInfos)
            {
                yield return TryResolveCollision(colliderComponent, collisionInfo, fDeltaTime);
            }
        }

        private IEnumerable<ColliderComponent> GetOverlappingColliders(Rect dynamicRect, ColliderComponent[] collidersToTest)
        {
            foreach (var collider in collidersToTest)
            {
                if (IsColliding(dynamicRect, collider.BoundingBox))
                    yield return collider;
            }
        }

        private ColliderComponent[] GetOverlappingColliders(ColliderComponent dynamicCollider, float deltaTime, ColliderComponent[] colliderComponents)
        {
            var movementBoundingBox = GetMovementBoundingBox(dynamicCollider, deltaTime);
            return GetOverlappingColliders(movementBoundingBox, colliderComponents)
                .ToArray();
        }

        /// <summary>
        /// Calculates the bounding box from current position to future position
        /// </summary>
        /// <param name="dynamicCollider"></param>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        private static Rect GetMovementBoundingBox(ColliderComponent dynamicCollider, float deltaTime)
        {
            var transform = dynamicCollider.GameObject.TransformComponent;
            var futurePosition = transform.Position + transform.Velocity * deltaTime;
            var unionDynamicRect = new Rect(futurePosition.X, futurePosition.Y, dynamicCollider.BoundingBox.Width, dynamicCollider.BoundingBox.Height);
            unionDynamicRect.Union(dynamicCollider.BoundingBox);
            return unionDynamicRect;
        }

        private bool IsRayInRect(Ray ray,
                                 Rect staticRect,
                                 out Vector2 contactPoint,
                                 out Vector2 contactNormal,
                                 out float contactTime)
        {
            contactPoint = Vector2.Zero;
            contactNormal = Vector2.Zero;
            contactTime = 0f;

            var targetPos = staticRect.TopLeft();
            var targetSize = staticRect.Size();

            // Calculate intersection with rectangle bounding axes
            var minContactPoint = (targetPos - ray.Origin) * ray.InvDirection;
            var maxContactPoint = (targetPos + targetSize - ray.Origin) * ray.InvDirection;

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

            // If negative it is pointing away from target
            if (maxIntersectionLength < 0) return false;

            // Contact point of collision from parametric line equation
            contactPoint = ray.Origin + contactTime * ray.Direction;

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

            //contactTime = Math.Clamp(contactTime, 0f, 1f);

            return true;
        }

        private bool IsRectInRect(Rect dynamicRect,
                                  Vector2 sourceVelocity,
                                  Rect staticRect,
                                  out Vector2 contactPoint,
                                  out Vector2 contactNormal,
                                  out float contactTime,
                                  float elapsedTime)
        {
            contactPoint = Vector2.Zero;
            contactNormal = Vector2.Zero;
            contactTime = 0f;

            if (sourceVelocity == Vector2.Zero) return false;

            var expandedStaticRect = staticRect.Add(dynamicRect.Size());

            var centerPoint = dynamicRect.TopLeft() + (dynamicRect.Size() / 2f);
            var ray = new Ray(centerPoint, sourceVelocity * elapsedTime);
            if (IsRayInRect(ray,
                            expandedStaticRect,
                            out contactPoint,
                            out contactNormal,
                            out contactTime))
            {
                if (contactTime >= 0f && contactTime < 1f) return true;
                else return false;
            }

            return false;
        }
    }
}
