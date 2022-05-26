using GamesLibrary.Utilities;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace GamesLibrary.Physics2D.Primitives
{
    /// <summary>
    /// Represents a ray. A ray has a starting point and a direction. The direction is presumed to be normalized.
    /// </summary>
    [DebuggerDisplay("(Origin:{Origin}, Direction:{Direction})")]
    public struct Ray2D : IEquatable<Ray2D>
    {
        public Ray2D(Vector2 origin, Vector2 direction)
        {
            Origin = origin;
            Direction = direction;
            InvDirection = Vector2.One / direction;
            Sign = new[] { Math.Sign(InvDirection.X), Math.Sign(InvDirection.Y) };
        }

        public Vector2 Origin { get; }
        public Vector2 Direction { get; }
        public Vector2 InvDirection { get; }
        public int[] Sign { get; }

        public bool Equals(Ray2D other)
        {
            return Origin == other.Origin && Direction == other.Direction && InvDirection == other.InvDirection;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Ray2D ray2D))
                return false;

            return Equals(ray2D);
        }

        public override int GetHashCode()
        {
            int hash = Origin.GetHashCode();
            hash = HashCodeHelper.CombineHashCodes(hash, Direction.GetHashCode());
            hash = HashCodeHelper.CombineHashCodes(hash, InvDirection.GetHashCode());
            hash = HashCodeHelper.CombineHashCodes(hash, Sign[0].GetHashCode());
            hash = HashCodeHelper.CombineHashCodes(hash, Sign[1].GetHashCode());
            return hash;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Ray2D left, Ray2D right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Ray2D left, Ray2D right)
        {
            return !(left == right);
        }
    }
}
