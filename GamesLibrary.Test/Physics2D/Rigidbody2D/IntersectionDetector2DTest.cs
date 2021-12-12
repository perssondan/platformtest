using GamesLibrary.Physics2D.Primitives;
using GamesLibrary.Physics2D.Rigidbody;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GamesLibrary.Test.Physics2D.Rigidbody
{
    public class IntersectionDetector2DTest
    {
        [TestCaseSource(nameof(AabbvsAabbTestData))]
        public void AABBCollisionTest(AABB b1, AABB b2, bool collision)
        {
            var hasCollision = IntersectionDetector2D.AABBAndAABBB(b1, b2);

            Assert.AreEqual(collision, hasCollision);
        }

        [TestCaseSource(nameof(AabbvsAabbTestData))]
        public void IsAABBColliding(AABB b1, AABB b2, bool collision)
        {
            var hasCollision = IntersectionDetector2D.IsAABBColliding(b1, b2);

            Assert.AreEqual(collision, hasCollision);
        }

        [TestCaseSource(nameof(RayVsAABBTestData))]
        public void IsRayColliding(Ray2D ray, AABB aabb, bool collision)
        {
            var collisionResult = IntersectionDetector2D.IsRayColliding(ray, aabb, out var collisionManifold);

            Assert.AreEqual(collision, collisionResult);
        }

        private static IEnumerable<TestCaseData> RayVsAABBTestData()
        {
            yield return new TestCaseData(
                new Ray2D(new Vector2(0, 0), new Vector2(2, 2)),
                new AABB(new Vector2(0, 0), new Vector2(2, 2)) { Rigidbody2D = new Rigidbody2D { Position = new Vector2(20, 0) } },
                false);
            yield return new TestCaseData(
                new Ray2D(new Vector2(0, 0), new Vector2(2, 2)),
                new AABB(new Vector2(0, 0), new Vector2(2, 2)) { Rigidbody2D = new Rigidbody2D { Position = new Vector2(2, 2) } },
                true);
        }

        private static IEnumerable<TestCaseData> AabbvsAabbTestData()
        {
            yield return new TestCaseData(
                new AABB(
                    new Vector2(0, 0), new Vector2(2, 2))
                {
                    Rigidbody2D = new Rigidbody2D()
                    {
                        Position = Vector2.Zero
                    }
                },
                new AABB(
                    new Vector2(0, 0), new Vector2(2, 2))
                {
                    Rigidbody2D = new Rigidbody2D()
                    {
                        Position = new Vector2(10, 10)
                    }
                },
                false);
            yield return new TestCaseData(
                new AABB(
                    new Vector2(0, 0), new Vector2(2, 2))
                {
                    Rigidbody2D = new Rigidbody2D()
                    {
                        Position = Vector2.Zero
                    }
                },
                new AABB(
                    new Vector2(0, 0), new Vector2(2, 2))
                {
                    Rigidbody2D = new Rigidbody2D()
                    {
                        Position = new Vector2(2, 2)
                    }
                },
                false);
            yield return new TestCaseData(
                new AABB(
                    new Vector2(0, 0), new Vector2(2, 2))
                {
                    Rigidbody2D = new Rigidbody2D()
                    {
                        Position = Vector2.Zero
                    }
                },
                new AABB(
                    new Vector2(0, 0), new Vector2(2, 2))
                {
                    Rigidbody2D = new Rigidbody2D()
                    {
                        Position = new Vector2(1f, 1f)
                    }
                },
                true);
        }

    }
}
