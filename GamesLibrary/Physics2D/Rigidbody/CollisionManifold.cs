using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace GamesLibrary.Physics2D.Rigidbody
{
    public class CollisionManifold
    {
        private HashSet<Vector2> _contactPoints = new HashSet<Vector2>();

        public CollisionManifold()
            : this(Vector2.Zero, Vector2.Zero, 0f, false)
        { }

        public CollisionManifold(Vector2 normal, Vector2 contactPoint, float time)
            : this(normal, contactPoint, time, true)
        { }

        protected CollisionManifold(Vector2 normal, Vector2 contactPoint, float time, bool isColliding)
        {
            Normal = normal;
            Time = time;
            IsColliding = isColliding;
            ContactPoint = contactPoint;
        }

        public Vector2 Normal { get; }

        public Vector2 ContactPoint { get; }

        public float Time { get; }

        public bool IsColliding { get; }

        public Vector2[] ContactPoints => _contactPoints.ToArray();

        public void AddContactPoint(Vector2 contactPoint)
        {
            _contactPoints.Add(contactPoint);
        }
    }
}
