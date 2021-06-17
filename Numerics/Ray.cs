using GamesLibrary.Utilities;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using uwpPlatformer.Components;

namespace uwpPlatformer.Numerics
{
    public struct Ray : IEquatable<Ray>
    {
        public Ray(Vector2 origin, Vector2 direction)
        {
            Origin = origin;
            Direction = direction;
            InvDirection = Vector2.One / direction;
            Sign = new[] { MathF.Sign(InvDirection.X), MathF.Sign(InvDirection.Y) };
        }

        public Vector2 Origin { get; }
        public Vector2 Direction { get; }
        public Vector2 InvDirection { get; }
        public int[] Sign { get; }

        public bool Equals(Ray other)
        {
            return Origin == other.Origin && Direction == other.Direction && InvDirection == other.InvDirection;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CollisionInfo collisionInfo))
                return false;

            return Equals(collisionInfo);
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
        public static bool operator ==(Ray left, Ray right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Ray left, Ray right)
        {
            return !(left == right);
        }
    }
}
