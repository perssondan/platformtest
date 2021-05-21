using System;
using System.Numerics;
using uwpPlatformer.GameObjects;
using Windows.Foundation;

namespace uwpPlatformer.Components
{
    public class ColliderComponent : GameObjectComponent, IComponent
    {
        public ColliderComponent(GameObject gameObject)
            : base(gameObject)
        {
            ColliderComponentManager.Instance.AddComponent(this);
        }

        public bool IsColliding { get; set; }

        public CollisionInfo[] CollisionInfos { get; set; } = Array.Empty<CollisionInfo>();

        /// <summary>
        /// Get or set <see cref="CollisionTypes"/>. Default value is <see cref="CollisionTypes.Static"/>
        /// </summary>
        public CollisionTypes CollisionType { get; set; } = CollisionTypes.Static;

        public Vector2 Size { get; set; } = Vector2.Zero;
        public Rect BoundingBox => new Rect(Position.ToPoint(), Size.ToSize());
        public Vector2 Center => new Vector2(Position.X + Size.X / 2f, Position.Y + Size.Y / 2f);
        public Vector2 LastValidPosition { get; set; } = Vector2.Zero;

        protected override void OnDispose()
        {
            ColliderComponentManager.Instance.RemoveComponent(this);
        }

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
}