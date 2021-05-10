using System;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using uwpKarate.GameObjects;
using Windows.Foundation;

namespace uwpKarate.Components
{
    public class ColliderComponent : GameObjectComponent, IGameObjectComponent
    {
        public ColliderComponent(GameObject gameObject)
            : base(gameObject)
        {
            ColliderComponentManager.Instance.AddComponent(this);
        }

        public bool IsColliding { get; set; }
        public bool IsGrounded { get; set; }

        public CollisionInfo[] CollisionInfos { get; set; } = Array.Empty<CollisionInfo>();

        /// <summary>
        /// Get or set <see cref="CollisionTypes"/>. Default value is <see cref="CollisionTypes.Static"/>
        /// </summary>
        public CollisionTypes CollisionType { get; set; } = CollisionTypes.Static;

        public Vector2 Size { get; set; } = Vector2.Zero;
        public Rect BoundingBox => new Rect(Position.ToPoint(), Size.ToSize());
        public Vector2 Center => new Vector2(Position.X + Size.X / 2f, Position.Y + Size.Y / 2f);
        public Vector2 LastValidPosition { get; set; } = Vector2.Zero;

        protected Vector2 Position => GameObject.TransformComponent.Position;

        public enum CollisionTypes
        {
            /// <summary>
            /// Value if the <see cref="GameObject"/> is static, cannot move
            /// </summary>
            Static,

            /// <summary>
            /// Value if the <see cref="GameObject"/> is dynamic, can move
            /// </summary>
            Dynamic
        }
    }

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

    internal static class HashCodeHelper
    {
        /// <summary>
        /// Combines two hash codes, useful for combining hash codes of individual vector elements
        /// </summary>
        internal static int CombineHashCodes(int h1, int h2)
        {
            return (((h1 << 5) + h1) ^ h2);
        }
    }
}