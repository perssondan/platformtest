using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Windows.Foundation;

namespace uwpKarate.Components
{
    public struct CollisionInfo : IEquatable<CollisionInfo>, IFormattable
    {
        public static CollisionInfo Zero => new CollisionInfo();

        public CollisionInfo(Vector2 collisionPoint, Vector2 collisionNormal, float collisionTime)
        {
            CollisionPoint = collisionPoint;
            CollisionNormal = collisionNormal;
            CollisionTime = collisionTime;
        }

        public Vector2 CollisionPoint;
        public Vector2 CollisionNormal;
        public float CollisionTime;
        public Rect ContactRect;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (!(obj is CollisionInfo))
                return false;
            return Equals((CollisionInfo)obj);
        }

        public override int GetHashCode()
        {
            int hash = CollisionPoint.GetHashCode();
            hash = HashCodeHelper.CombineHashCodes(hash, CollisionNormal.GetHashCode());
            hash = HashCodeHelper.CombineHashCodes(hash, CollisionTime.GetHashCode());
            return hash;
        }

        public bool Equals(CollisionInfo other)
        {
            return CollisionNormal == other.CollisionNormal && CollisionPoint == other.CollisionPoint && CollisionTime == other.CollisionTime;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"{nameof(CollisionNormal)}:{CollisionNormal};{nameof(CollisionPoint)}:{CollisionPoint};{nameof(CollisionTime)}:{CollisionTime}";
        }

        public override string ToString()
        {
            return $"{nameof(CollisionNormal)}:{CollisionNormal};{nameof(CollisionPoint)}:{CollisionPoint};{nameof(CollisionTime)}:{CollisionTime}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(CollisionInfo left, CollisionInfo right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(CollisionInfo left, CollisionInfo right)
        {
            return !(left == right);
        }
    }
}