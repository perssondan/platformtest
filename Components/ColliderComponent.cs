using System;
using System.Linq;
using System.Numerics;
using uwpKarate.Extensions;
using uwpKarate.GameObjects;
using Windows.Foundation;

namespace uwpKarate.Components
{
    public class ColliderComponent : GameObjectComponent, IGameObjectComponent<World>
    {
        private World _world;
        public ColliderComponent(GameObject gameObject)
            : base(gameObject)
        {
        }

        public bool IsColliding { get; set; }
        public Vector2 Size { get; set; }
        public Rect Rect => new Rect(GameObject.TransformComponent.Position.ToPoint(), Size.ToSize());

        public void Update(World target, TimeSpan timeSpan)
        {
            _world = target;
        }

        public bool IsPointColliding(Vector2 point)
        {
            return IsColliding = Rect.Contains(point.ToPoint());
        }

        //public bool IsPointInRect(Vector2 point, Rect rect)
        //{
        //    return rect.Contains(point.ToPoint());
        //}

        public bool IsCollidingTest(Vector2 oldPosition, Vector2 newPosition, out Vector2 resolvedPosition)
        {
            IsColliding = false;
            resolvedPosition.Y = oldPosition.Y;
            resolvedPosition.X = newPosition.X;
            if (_world is null) return false;

            if (newPosition.X > oldPosition.X) // going right
            {
                if (_world.TryGetOverlappingTiles(resolvedPosition.ToRect(32f, 32f), out var rects))
                {
                    var minimumX = rects.Min(rect => rect.Left);
                    if (resolvedPosition.X + 32f > minimumX)
                    {
                        resolvedPosition.X = (float)minimumX - 32f;
                        if (resolvedPosition.X < 0)
                            resolvedPosition.X = 0f;
                        IsColliding = true;
                    }
                }
            }
            else if (newPosition.X < oldPosition.X) // going left
            {
                if (_world.TryGetOverlappingTiles(resolvedPosition.ToRect(32f, 32f), out var rects))
                {
                    var maximumX = rects.Max(rect => rect.Right);
                    if (resolvedPosition.X < maximumX)
                    {
                        resolvedPosition.X = (float)maximumX;
                        IsColliding = true;
                    }
                }
            }

            resolvedPosition.Y =  newPosition.Y;
            if (newPosition.Y > oldPosition.Y) // going down
            {
                if (_world.TryGetOverlappingTiles(resolvedPosition.ToRect(32f, 32f), out var rects))
                {
                    var minimumY = rects.Min(rect => rect.Top);
                    if (resolvedPosition.Y + 32f > minimumY)
                    {
                        resolvedPosition.Y = (float)minimumY - 32f;
                        IsColliding = true;
                    }
                }
            }
            else if (newPosition.Y < oldPosition.Y) // going up
            {
                if (_world.TryGetOverlappingTiles(resolvedPosition.ToRect(32f, 32f), out var rects))
                {
                    var maximumY = rects.Max(rect => rect.Bottom);
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

        public bool IsRectColliding(Rect opponentRect)
        {
            return IsColliding = (Rect.Left < opponentRect.Right && Rect.Right > opponentRect.Left &&
                    Rect.Top < opponentRect.Bottom && Rect.Bottom > opponentRect.Top);
        }

        public bool IsRayInRect(Vector2 rayOrigin,
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

            var targetPos = staticRect.Pos();
            var targetSize = staticRect.Size();

            // Calculate intersection with rectangle bounding axes
            var nearestContactPoint = (targetPos - rayOrigin) * inverseRayDirection;
            var furthestContactPoint = (targetPos + targetSize - rayOrigin) * inverseRayDirection;

            if (float.IsNaN(furthestContactPoint.X) || float.IsNaN(furthestContactPoint.Y)) return false;
            if (float.IsNaN(nearestContactPoint.X) || float.IsNaN(nearestContactPoint.Y)) return false;

            // Swap
            if (nearestContactPoint.X > furthestContactPoint.X) ObjectExtensions.Swap(ref nearestContactPoint.X, ref furthestContactPoint.X);

            // Swap
            if (nearestContactPoint.Y > furthestContactPoint.Y) ObjectExtensions.Swap(ref nearestContactPoint.Y, ref furthestContactPoint.Y);

            if (nearestContactPoint.X > furthestContactPoint.Y || nearestContactPoint.Y > furthestContactPoint.X) return false;

            // Nearest 'time' will be the first contact
            nearestContactTime = Math.Max(nearestContactPoint.X, nearestContactPoint.Y);

            // Furthest 'time' will contact on the opposite side of the target
            var furhestContactTime = Math.Min(furthestContactPoint.X, nearestContactPoint.Y);

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

        private bool IsGrounded(World world)
        {
            return false;
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

            var expandedTarger = staticRect.Add(dynamicRect.Size());

            if (IsRayInRect(
                            dynamicRect.Pos() + dynamicRect.Size() / 2f,
                            sourceVelocity * elapsedTime,
                            expandedTarger,
                            out contactPoint,
                            out contactNormal,
                            out contactTime))
            {
                if (contactTime >= 0f && contactTime <= 1f) return true;
            }

            return false;
        }
    }
}