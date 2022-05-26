using GamesLibrary.Utilities;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace GamesLibrary.Physics2D.Primitives
{
    [DebuggerDisplay("(StartPoint:{StartPoint}, EndPoint:{EndPoint})")]
    public struct LineSegment : IEquatable<LineSegment>
    {
        public LineSegment(Vector2 startPoint, Vector2 endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;

            Direction = endPoint - startPoint;
            InvDirection = Vector2.One / Direction;
            Sign = new[] { Math.Sign(InvDirection.X), Math.Sign(InvDirection.Y) };
        }

        public Vector2 StartPoint { get; }

        public Vector2 EndPoint { get; }

        public Vector2 Direction { get; }

        public Vector2 InvDirection { get; }

        public int[] Sign { get; }

        public bool Equals(LineSegment other)
        {
            return StartPoint == other.StartPoint && EndPoint == other.EndPoint;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is LineSegment lineSegment))
                return false;

            return Equals(lineSegment);
        }

        public override int GetHashCode()
        {
            int hash = StartPoint.GetHashCode();
            hash = HashCodeHelper.CombineHashCodes(hash, EndPoint.GetHashCode());
            return hash;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(LineSegment left, LineSegment right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(LineSegment left, LineSegment right)
        {
            return !(left == right);
        }
    }
}
