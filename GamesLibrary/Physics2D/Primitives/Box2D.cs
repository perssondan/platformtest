using GamesLibrary.Physics2D.Rigidbody;
using System.Numerics;

namespace GamesLibrary.Physics2D.Primitives
{
    public class Box2D
    {
        private Vector2 _size;

        public Box2D()
            : this(Vector2.Zero, Vector2.Zero)
        {
        }

        public Box2D(Vector2 min, Vector2 max)
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

        public Vector2 LocalMin => Rigidbody2D?.Position ?? Vector2.Zero - HalfSize;

        public Vector2 LocalMax => Rigidbody2D?.Position ?? Vector2.Zero + HalfSize;

        public Rigidbody2D Rigidbody2D { get; set; }

        public Vector2[] GetVertices()
        {
            var min = LocalMin;
            var max = LocalMax;

            var vertices = new[]
            {
                new Vector2(min.X, min.Y), new Vector2(min.X, max.Y),
                new Vector2(max.X, min.Y), new Vector2(max.Y, max.Y)
            };

            if (Rigidbody2D.Rotation != 0.0f)
            {
                for (var index = 0; index < vertices.Length; index++)
                {
                    vertices[index] = Vector2.Transform(vertices[index], Matrix3x2.CreateRotation(Rigidbody2D?.Rotation ?? 0f, Rigidbody2D?.Position ?? Vector2.Zero));
                    index++;
                }
            }

            return vertices;
        }
    }
}
