using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using uwpKarate.Components;
using uwpKarate.Extensions;
using uwpKarate.GameObjects;
using Windows.Foundation;

namespace uwpKarate.Systems
{
    public class ColliderSystem : SystemBase<ColliderSystem>
    {
        public event Action<GameObject, bool> GroundedEvent;
        public event Action<GameObject, bool> CollidedEvent;

        public override void Update(World world, TimeSpan deltaTime)
        {
            var dynamicColliders = ColliderComponentManager.Instance.Components
                .Where(collider => collider.CollisionType == ColliderComponent.CollisionTypes.Dynamic);
            if (!dynamicColliders.Any()) return;

            dynamicColliders.ForEach(dynamicCollider =>
            {
                dynamicCollider.IsColliding = false;
                dynamicCollider.IsGrounded = false;

                var isColliding = IsColliding(dynamicCollider, (float)deltaTime.TotalSeconds);

                dynamicCollider.IsColliding = isColliding;
                dynamicCollider.IsGrounded = isColliding;

                GroundedEvent?.Invoke(dynamicCollider.GameObject, dynamicCollider.IsGrounded);
                CollidedEvent?.Invoke(dynamicCollider.GameObject, dynamicCollider.IsColliding);
            });
            

            //  TODO: Investigate QuadTree

            

            //foreach (var collider in colliderComponents)
            //{
            //    var isGrounded = IsGrounded(playerCollider, collider);
            //    if (isGrounded)
            //    {
            //        playerCollider.IsColliding = true;
            //        playerCollider.IsGrounded = true;
            //    }
            //    collider.IsColliding = IsColliding(playerCollider, collider);
            //    // A static collider is never grounded
            //    collider.IsGrounded = collider.CollisionType == ColliderComponent.CollisionTypes.Static ? false : isGrounded;
            //}

            //foreach(var collider in ColliderComponentManager.Instance.Components)
            //{
            //    collider.LastValidPosition = collider.GameObject.TransformComponent.Position;
            //}
        }

        public bool TryResolveCollision(ColliderComponent colliderComponent, CollisionInfo collisionInfo, float deltaTime)
        {
            var transformComponent = colliderComponent.GameObject.TransformComponent;
            if (IsRectInRect(colliderComponent.BoundingBox, transformComponent.Velocity, collisionInfo.ContactRect, out var contactPoint, out var contactNormal, out var contactTime, deltaTime))
            {
                //if (contactTime >= 0f && contactTime < 1f)
                {
                    transformComponent.Velocity += contactNormal * new Vector2(Math.Abs(transformComponent.Velocity.X), Math.Abs(transformComponent.Velocity.Y)) * (1f - contactTime);
                    return true;
                }
            }

            return false;
        }

        private bool IsGrounded(ColliderComponent first, ColliderComponent second)
        {
            var bottomLeft = new Vector2((float)first.BoundingBox.X, (float)first.BoundingBox.Bottom);

            // we only check a box of 1pxl height if checking for is grounded
            var bottomRect = bottomLeft.ToRect(first.Size.X, 1f);
            return IsColliding(bottomRect, second.BoundingBox);
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

        private IEnumerable<ColliderComponent> GetOverlappingColliders(Rect dynamicRect, ColliderComponent[] collidersToTest)
        {
            foreach (var collider in collidersToTest)
            {
                if (IsColliding(dynamicRect, collider.BoundingBox))
                    yield return collider;
            }
        }

        private IEnumerable<ColliderComponent> GetOverlappingColliders(ColliderComponent targetCollider, ColliderComponent[] collidersToTest)
        {
            return GetOverlappingColliders(targetCollider.BoundingBox, collidersToTest);
        }

        private bool IsGrounded(World world, GameObject gameObject)
        {
            var bottomLeft = new Vector2(gameObject.TransformComponent.Position.X, gameObject.TransformComponent.Position.Y + gameObject.ColliderComponent.Size.Y);

            var bottomRect = bottomLeft.ToRect(gameObject.ColliderComponent.Size.X, 1f);
            if (world?.TryGetOverlappingTiles(bottomRect, out _) == true) return true;

            return false;
        }

        private bool IsCollidingTest(World world, Vector2 oldPosition, Vector2 newPosition, Vector2 size, out Vector2 resolvedPosition)
        {
            var IsColliding = false;
            resolvedPosition.Y = oldPosition.Y;
            resolvedPosition.X = newPosition.X;
            if (world is null) return false;

            if (newPosition.X > oldPosition.X) // going right
            {
                if (world.TryGetOverlappingTiles(resolvedPosition.ToRect(size.X, size.Y), out var collidingRects))
                {
                    var minimumX = collidingRects.Min(rect => rect.Left);
                    if (resolvedPosition.X + size.X > minimumX)
                    {
                        resolvedPosition.X = (float)minimumX - size.X;
                        if (resolvedPosition.X < 0)
                            resolvedPosition.X = 0f;
                        IsColliding = true;
                    }
                }
            }
            else if (newPosition.X < oldPosition.X) // going left
            {
                if (world.TryGetOverlappingTiles(resolvedPosition.ToRect(size.X, size.Y), out var collidingRects))
                {
                    var maximumX = collidingRects.Max(rect => rect.Right);
                    if (resolvedPosition.X < maximumX)
                    {
                        resolvedPosition.X = (float)maximumX;
                        IsColliding = true;
                    }
                }
            }

            resolvedPosition.Y = newPosition.Y;
            if (newPosition.Y > oldPosition.Y) // going down
            {
                if (world.TryGetOverlappingTiles(resolvedPosition.ToRect(size.X, size.Y), out var collidingRects))
                {
                    var minimumY = collidingRects.Min(rect => rect.Top);
                    if (resolvedPosition.Y + size.Y > minimumY)
                    {
                        resolvedPosition.Y = (float)minimumY - size.Y;
                        IsColliding = true;
                    }
                }
            }
            else if (newPosition.Y < oldPosition.Y) // going up
            {
                if (world.TryGetOverlappingTiles(resolvedPosition.ToRect(size.X, size.Y), out var collidingRects))
                {
                    var maximumY = collidingRects.Max(rect => rect.Bottom);
                    {
                        if (resolvedPosition.Y < maximumY)
                        {
                            resolvedPosition.Y = (float)maximumY;
                            IsColliding = true;
                        }
                    }
                }
            }

            return IsColliding;
        }

        private bool IsRayInRect(Vector2 rayOrigin,
                                 Vector2 rayDirection,
                                 Rect staticRect,
                                 out Vector2 contactPoint,
                                 out Vector2 contactNormal,
                                 out float nearestContactTime)
        {
            contactPoint = Vector2.Zero;
            contactNormal = Vector2.Zero;
            nearestContactTime = 0f;

            // Cache division
            var inverseRayDirection = Vector2.One / rayDirection;
            //olc::vf2d invdir = 1.0f / ray_dir;

            var targetPos = staticRect.Pos();
            var targetSize = staticRect.Size();

            // Calculate intersection with rectangle bounding axes
            var nearestContactPoint = (targetPos - rayOrigin) * inverseRayDirection;
            //olc::vf2d t_near = (target->pos - ray_origin) * invdir;
            var furthestContactPoint = (targetPos + targetSize - rayOrigin) * inverseRayDirection;
            //olc::vf2d t_far = (target->pos + target->size - ray_origin) * invdir;

            if (float.IsNaN(furthestContactPoint.X) || float.IsNaN(furthestContactPoint.Y)) return false;
            //if (std::isnan(t_far.y) || std::isnan(t_far.x)) return false;
            if (float.IsNaN(nearestContactPoint.X) || float.IsNaN(nearestContactPoint.Y)) return false;
            //if (std::isnan(t_near.y) || std::isnan(t_near.x)) return false;

            // Swap
            if (nearestContactPoint.X > furthestContactPoint.X) ObjectExtensions.Swap(ref nearestContactPoint.X, ref furthestContactPoint.X);
            //if (t_near.x > t_far.x) std::swap(t_near.x, t_far.x);

            // Swap
            if (nearestContactPoint.Y > furthestContactPoint.Y) ObjectExtensions.Swap(ref nearestContactPoint.Y, ref furthestContactPoint.Y);
            //if (t_near.y > t_far.y) std::swap(t_near.y, t_far.y);

            if (nearestContactPoint.X > furthestContactPoint.Y || nearestContactPoint.Y > furthestContactPoint.X) return false;
            //if (t_near.x > t_far.y || t_near.y > t_far.x) return false;

            // Nearest 'time' will be the first contact
            nearestContactTime = Math.Max(nearestContactPoint.X, nearestContactPoint.Y);
            //t_hit_near = std::max(t_near.x, t_near.y);

            // Furthest 'time' will contact on the opposite side of the target
            var furhestContactTime = Math.Min(furthestContactPoint.X, furthestContactPoint.Y);
            //float t_hit_far = std::min(t_far.x, t_far.y);

            // If negative it is pointing away from target
            if (furhestContactTime < 0) return false;

            // Contact point of collision from parametric line equation
            contactPoint = rayOrigin + nearestContactTime * rayDirection;

            if (nearestContactPoint.X > nearestContactPoint.Y)
            {
                if (inverseRayDirection.X < 0f)
                {
                    contactNormal = new Vector2(1, 0);
                }
                else
                {
                    contactNormal = new Vector2(-1, 0);
                }
            }
            else if (nearestContactPoint.X < nearestContactPoint.Y)
            {
                if (inverseRayDirection.Y < 0f)
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

        public bool IsColliding(ColliderComponent dynamicCollider, float deltaTime)
        {
            dynamicCollider.CollisionInfos = Array.Empty<CollisionInfo>();

            if (dynamicCollider.CollisionType == ColliderComponent.CollisionTypes.Static) return false;

            var colliderComponents = ColliderComponentManager.Instance.Components.Where(collider => collider.CollisionType == ColliderComponent.CollisionTypes.Static).ToArray();

            colliderComponents.ForEach(collider => collider.IsColliding = false);

            var newPosition = dynamicCollider.GameObject.TransformComponent.Position + dynamicCollider.GameObject.TransformComponent.Velocity * deltaTime;
            var unionDynamicRect = new Rect(newPosition.X, newPosition.Y, dynamicCollider.BoundingBox.Width, dynamicCollider.BoundingBox.Height);
            unionDynamicRect.Union(dynamicCollider.BoundingBox);
            var componentsInCollision = GetOverlappingColliders(unionDynamicRect, colliderComponents).ToArray();
            var componentsInCollisionWithOrigin = GetOverlappingColliders(dynamicCollider.BoundingBox, colliderComponents).ToArray();

            var wantedVelocity = dynamicCollider.GameObject.TransformComponent.Velocity;
            var results = new List<CollisionInfo>();
            foreach (var componentInCollision in componentsInCollision)
            {
                if (IsRectInRect(dynamicCollider.BoundingBox, dynamicCollider.GameObject.TransformComponent.Velocity, componentInCollision.BoundingBox, out var cContactPoint, out var cContactNormal, out var cContactTime, deltaTime))
                {
                    var collisionInfo = new CollisionInfo(cContactPoint, cContactNormal, cContactTime);
                    collisionInfo.ContactRect = componentInCollision.BoundingBox;
                    results.Add(collisionInfo);
                    componentInCollision.IsColliding = true;
                }
            }

            var isMovingHorizontally = dynamicCollider.GameObject.TransformComponent.Velocity.X != 0f;

            if (results.Any())
            {
                dynamicCollider.CollisionInfos = results.OrderBy(info => info.CollisionTime).ToArray();
                return true;
            }

            return false;
        }

        public bool IsRectInRect(Rect dynamicRect,
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

            var expandedTarger = staticRect.Add(dynamicRect.Size());

            if (IsRayInRect(
                            dynamicRect.Pos() + dynamicRect.Size() / 2f,
                            sourceVelocity * elapsedTime,
                            expandedTarger,
                            out contactPoint,
                            out contactNormal,
                            out contactTime))
            {
                if (contactTime >= 0f && contactTime < 1f) return true;
            }

            return false;
        }
    }
}