using GamesLibrary.Physics2D.Rigidbody;
using System.Numerics;

namespace GamesLibrary.Physics2D.Primitives
{
    public class AABB
    {
        private Vector2 _size;

        public AABB()
            : this(Vector2.Zero, Vector2.Zero)
        {
        }

        public AABB(Vector2 min, Vector2 max)
        {
            Size = max - min;
        }

        public Vector2 Size
        {
            get => _size;
            set
            {
                _size = value;
                HalfSize = _size * 0.5f;
            }
        }

        public Vector2 HalfSize { get; private set; }

        public Vector2 Min => (Rigidbody2D?.Position ?? Vector2.Zero) - HalfSize;

        public Vector2 Max => (Rigidbody2D?.Position ?? Vector2.Zero) + HalfSize;

        public Rigidbody2D Rigidbody2D { get; set; }
    }
}
