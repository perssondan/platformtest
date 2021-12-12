using System.Numerics;

namespace GamesLibrary.Physics2D.Primitives
{
    public class Ray2D
    {
        public Ray2D(Vector2 origin, Vector2 direction)
        {
            Origin = origin;
            Direction = Vector2.Normalize(direction);
            InvDirection = Vector2.One / direction;
        }

        public Vector2 Origin { get; }

        public Vector2 Direction { get; }
        public Vector2 InvDirection { get; }
    }
}
