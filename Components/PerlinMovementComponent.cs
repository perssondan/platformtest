using GamesLibrary.Utilities;
using System.Runtime.CompilerServices;
using uwpPlatformer.GameObjects;
using Windows.Foundation;

namespace uwpPlatformer.Components
{
    public struct PerlinMovementComponent : IComponent
    {
        public PerlinMovementComponent(GameObject gameObject, Rect bounds, float offsetX, float offsetY)
        {
            GameObject = gameObject;
            Bounds = bounds;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        public GameObject GameObject { get; set; }
        public Rect Bounds;
        public float OffsetX;
        public float OffsetY;

        public void Dispose()
        {
        }

        public override bool Equals(object obj)
        {
            return obj is PerlinMovementComponent component &&
                   GetHashCode() == component.GetHashCode();
        }

        public override int GetHashCode()
        {
            int hashCode = GameObject?.GetHashCode() ?? 32432423;
            hashCode = HashCodeHelper.CombineHashCodes(hashCode, Bounds.GetHashCode());

            return hashCode;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(PerlinMovementComponent left, PerlinMovementComponent right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(PerlinMovementComponent left, PerlinMovementComponent right)
        {
            return !(left == right);
        }
    }
}
