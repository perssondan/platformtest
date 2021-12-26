using GamesLibrary.Utilities;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Windows.Foundation;

namespace uwpPlatformer.Components
{
    [DebuggerDisplay("CollisionPoint:{CollisionPoint}, CollisionNormal:{CollisionNormal}, CollisionTime:{CollisionTime}, ContactRect:{ContactRect}")]
    public struct CollisionManifold : IEquatable<CollisionManifold>, IFormattable
    {
        public static CollisionManifold Zero => new CollisionManifold();

        public CollisionManifold(Vector2 collisionPoint, Vector2 collisionNormal, float collisionTime, Rect contactRect)
        {
            CollisionPoint = collisionPoint;
            CollisionNormal = collisionNormal;
            CollisionTime = collisionTime;
            ContactRect = contactRect;
        }

        public Vector2 CollisionPoint;
        public Vector2 CollisionNormal;
        public float CollisionTime;
        public Rect ContactRect;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (!(obj is CollisionManifold))
                return false;
            return Equals((CollisionManifold)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hash = CollisionPoint.GetHashCode();
            hash = HashCodeHelper.CombineHashCodes(hash, CollisionNormal.GetHashCode());
            hash = HashCodeHelper.CombineHashCodes(hash, CollisionTime.GetHashCode());
            return hash;
        }

        /// <inheritdoc />
        public bool Equals(CollisionManifold other)
        {
            return CollisionNormal == other.CollisionNormal && CollisionPoint == other.CollisionPoint && CollisionTime == other.CollisionTime;
        }

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"{nameof(CollisionNormal)}:{CollisionNormal};{nameof(CollisionPoint)}:{CollisionPoint};{nameof(CollisionTime)}:{CollisionTime}";
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(CollisionNormal)}:{CollisionNormal};{nameof(CollisionPoint)}:{CollisionPoint};{nameof(CollisionTime)}:{CollisionTime}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(CollisionManifold left, CollisionManifold right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(CollisionManifold left, CollisionManifold right)
        {
            return !(left == right);
        }
    }
}