using GamesLibrary.Physics2D.Primitives;
using System;
using System.Numerics;

namespace GamesLibrary.Physics2D.Rigidbody
{
    public class IntersectionDetector2D
    {
        public static bool AABBAndAABBB(AABB b1, AABB b2)
        {
            var axesToTest = new[] { new Vector2(0, 1), new Vector2(1, 0) };
            for (int i = 0; i < axesToTest.Length; i++)
            {
                if (!OverlapOnAxis(b1, b2, axesToTest[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsAABBColliding(AABB sourceRect, AABB opponentRect)
        {
            return (sourceRect.Min.X < opponentRect.Max.X && sourceRect.Max.X > opponentRect.Min.X) &&
                    (sourceRect.Min.Y < opponentRect.Max.Y && sourceRect.Max.Y > opponentRect.Min.Y);
        }

        public static bool IsRayColliding(Ray2D ray, AABB target, out CollisionManifold collisionManifold)
        {
            collisionManifold = new CollisionManifold();
            var contactNormal = Vector2.Zero;

            var minPos = target.Min;
            var maxPos = target.Max;

            // Calculate intersection with rectangle bounding axes
            var minContactPoint = (minPos - ray.Origin) * ray.InvDirection;
            var maxContactPoint = (maxPos - ray.Origin) * ray.InvDirection;

            if (float.IsNaN(maxContactPoint.X) || float.IsNaN(maxContactPoint.Y)) return false;
            if (float.IsNaN(minContactPoint.X) || float.IsNaN(minContactPoint.Y)) return false;

            // Swap
            if (minContactPoint.X > maxContactPoint.X)
            {
                var temp = minContactPoint.X;
                minContactPoint.X = maxContactPoint.X;
                maxContactPoint.X = temp;
            }

            // Swap
            if (minContactPoint.Y > maxContactPoint.Y)
            {
                var temp = minContactPoint.Y;
                minContactPoint.Y = maxContactPoint.Y;
                maxContactPoint.Y = temp;
            }

            if (minContactPoint.X > maxContactPoint.Y || minContactPoint.Y > maxContactPoint.X) return false;

            // Min 'time' will be the first contact
            var contactTime = Math.Max(minContactPoint.X, minContactPoint.Y);

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
            var contactPoint = ray.Origin + contactTime * ray.Direction;

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
            else
            {

            }

            collisionManifold = new CollisionManifold(contactNormal, contactPoint, contactTime);

            return true;
        }

        //public static boolean raycast(AABB box, Ray2D ray, RaycastResult result)
        //{
        //    RaycastResult.reset(result);

        //    Vector2f unitVector = ray.getDirection();
        //    unitVector.normalize();
        //    unitVector.x = (unitVector.x != 0) ? 1.0f / unitVector.x : 0f;
        //    unitVector.y = (unitVector.y != 0) ? 1.0f / unitVector.y : 0f;

        //    Vector2f min = box.getMin();
        //    min.sub(ray.getOrigin()).mul(unitVector);
        //    Vector2f max = box.getMax();
        //    max.sub(ray.getOrigin()).mul(unitVector);

        //    float tmin = Math.max(Math.min(min.x, max.x), Math.min(min.y, max.y));
        //    float tmax = Math.min(Math.max(min.x, max.x), Math.max(min.y, max.y));
        //    if (tmax < 0 || tmin > tmax)
        //    {
        //        return false;
        //    }

        //    float t = (tmin < 0f) ? tmax : tmin;
        //    boolean hit = t > 0f; //&& t * t < ray.getMaximum();
        //    if (!hit)
        //    {
        //        return false;
        //    }

        //    if (result != null)
        //    {
        //        Vector2f point = new Vector2f(ray.getOrigin()).add(
        //                new Vector2f(ray.getDirection()).mul(t));
        //        Vector2f normal = new Vector2f(ray.getOrigin()).sub(point);
        //        normal.normalize();

        //        result.init(point, normal, t, true);
        //    }

        //    return true;
        //}

        private static bool OverlapOnAxis(AABB b1, AABB b2, Vector2 axis)
        {
            var interval1 = GetInterval(b1, axis);
            var interval2 = GetInterval(b2, axis);
            return ((interval2.X <= interval1.Y) && (interval1.X <= interval2.Y));
        }

        private static Vector2 GetInterval(AABB rect, Vector2 axis)
        {
            var result = new Vector2(0, 0);

            var min = rect.Min;
            var max = rect.Max;

            var vertices = new[]
            {
                new Vector2(min.X, min.Y), new Vector2(min.X, max.Y),
                new Vector2(max.X, min.Y), new Vector2(max.X, max.Y)
            };

            result.X = Vector2.Dot(axis, vertices[0]);
            result.Y = result.X;
            for (int i = 1; i < 4; i++)
            {
                var projection = Vector2.Dot(axis, vertices[i]);
                if (projection < result.X)
                {
                    result.X = projection;
                }
                if (projection > result.Y)
                {
                    result.Y = projection;
                }
            }
            return result;
        }
    }
}
