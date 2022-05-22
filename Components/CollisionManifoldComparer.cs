using System.Collections.Generic;

namespace uwpPlatformer.Components
{
    public class CollisionManifoldComparer : IComparer<CollisionManifold>
    {
        /// <inheritdoc />
        public int Compare(CollisionManifold x, CollisionManifold y)
        {
            return x.CollisionTime.CompareTo(y.CollisionTime);
        }
    }
}
