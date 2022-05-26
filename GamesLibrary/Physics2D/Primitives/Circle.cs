using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace GamesLibrary.Physics2D.Primitives
{
    public struct Circle
    {
        public Circle(Vector2 center, float radius = 0f)
        {
            Radius = radius;
            RadiusSquared = radius * radius;
            Center = center;
        }

        public float Radius { get; }
        public Vector2 Center { get; }
        public float RadiusSquared { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Circle left, Circle right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Circle left, Circle right)
        {
            return !(left == right);
        }
    }
}
