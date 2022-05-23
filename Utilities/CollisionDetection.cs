using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using uwpPlatformer.Extensions;
using uwpPlatformer.Numerics;
using Windows.Foundation;

namespace uwpPlatformer.Utilities
{
    public static class CollisionDetection
    {
        public static bool IntersectAABB(BoundingBox dynamicRect, BoundingBox staticRect, out Vector2 normal, out Vector2 contactPoint)
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
                contactPoint.Y = (float)dynamicRect.Center.Y;
            }
            else
            {
                var sy = Math.Sign(dy);
                normal.Y = sy;
                contactPoint.X = (float)dynamicRect.Center.X;
                contactPoint.Y = (float)staticRect.Top + (staticRect.Half().Y * sy);
            }

            return true;
        }

        public static bool IsAABBColliding(BoundingBox sourceRect, BoundingBox opponentRect)
        {
            return (sourceRect.Left < opponentRect.Right && sourceRect.Right > opponentRect.Left) &&
                    (sourceRect.Top < opponentRect.Bottom && sourceRect.Bottom > opponentRect.Top);
        }

        public static bool AABBvsAABB(BoundingBox first, BoundingBox other)
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

        public static bool IsRectInRect(BoundingBox dynamicRect,
                                        Vector2 newPosition,
                                        BoundingBox staticRect,
                                        out Vector2 contactPoint,
                                        out Vector2 contactNormal,
                                        out float contactTime)
        {
            var originPosition = dynamicRect.Position;
            var centerPoint = dynamicRect.Center;
            var centerOffset = centerPoint - originPosition;
            var newCenterPoint = newPosition + centerOffset;

            var expandedStaticRect = staticRect.Add(dynamicRect);

            var lineSegment = new LineSegment(centerPoint, newCenterPoint);

            var isRayInRect = IsRayInRect(lineSegment,
                            expandedStaticRect,
                            out contactPoint,
                            out contactNormal,
                            out contactTime);

            return isRayInRect && contactTime >= 0f && contactTime < 1f;
        }

        public static bool IsRayInRect(LineSegment ray,
                                       BoundingBox staticRect,
                                       out Vector2 contactPoint,
                                       out Vector2 contactNormal,
                                       out float contactTime)
        {
            contactPoint = Vector2.Zero;
            contactNormal = Vector2.Zero;
            contactTime = 0f;

            var minPos = staticRect.Position;
            var maxPos = minPos + staticRect.Size;

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
    }
}
