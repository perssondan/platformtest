using System;
using System.Numerics;
using uwpPlatformer.GameObjects;
using uwpPlatformer.Numerics;
using Windows.Foundation;

namespace uwpPlatformer.Components
{
    public class ColliderComponent : ComponentBase, IComponent
    {
        public ColliderComponent(GameObject gameObject)
            : base(gameObject)
        {
        }

        public bool IsColliding { get; set; }

        public CollisionManifold[] CollisionManifolds { get; set; } = Array.Empty<CollisionManifold>();

        /// <summary>
        /// Get or set <see cref="CollisionTypes"/>. Default value is <see cref="CollisionTypes.StaticPlatform"/>
        /// </summary>
        public CollisionTypes CollisionType { get; set; } = CollisionTypes.StaticPlatform;

        public Vector2 Size { get; set; } = Vector2.Zero;
        public Rect BoundingBox => new Rect(Position.ToPoint(), Size.ToSize());
        public Vector2 Center => new Vector2(Position.X + Size.X / 2f, Position.Y + Size.Y / 2f);

        public Rect MovingBoundingBox { get; set; } = Rect.Empty;

        protected Vector2 Position => GameObject.TransformComponent.Position;

        [Flags]
        public enum CollisionTypes
        {
            /// <summary>
            /// No collision
            /// </summary>
            None = 0x00,

            /// <summary>
            /// Value if the <see cref="GameObject"/> is static, cannot move
            /// </summary>
            StaticPlatform = 0b000001,

            /// <summary>
            /// Part of the world boundary
            /// </summary>
            StaticWorld = 0b000010,

            IsStaticMask = 0b000011,

            /// <summary>
            /// Value if the <see cref="GameObject"/> is dynamic, can move
            /// </summary>
            Dynamic = 0b001000,

            /// <summary>
            /// This together with another type that yields > 0 should be collision evaluated
            /// </summary>
            DynamicMask = Dynamic | DynamicWorld | StaticWorld | StaticPlatform,

            /// <summary>
            /// Only interacts with static world and dynamic
            /// </summary>
            DynamicWorld = 0b010000,

            /// <summary>
            /// This together with another type that yields > 0 should be collision evaluated
            /// </summary>
            DynamicWorldMask = Dynamic | DynamicWorld | StaticWorld,

            /// <summary>
            /// This together with another type that yields > 0 is dynamic
            /// </summary>
            IsDynamicMask = Dynamic | DynamicWorld,
        }
    }
}
