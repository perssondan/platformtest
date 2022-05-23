using System;
using uwpPlatformer.GameObjects;
using uwpPlatformer.Numerics;
using Windows.Foundation;

namespace uwpPlatformer.Components
{
    public class PlayerComponent : ComponentBase, IComponent
    {
        public PlayerComponent(GameObject gameObject)
            : base(gameObject)
        {
        }

        public TimeSpan JumpPressedRememberTime => TimeSpan.FromMilliseconds(150);
        public TimeSpan JumpPressedAt { get; set; }
        public bool IsJumpButtonPressed { get; set; }

        // TODO: Should be fetched from tile file
        public BoundingBox[] WalkSourceRects => new BoundingBox[]
            {
                new BoundingBox(0, 32f, 32f, 32f),
                new BoundingBox(32f, 32f, 32f, 32f),
                new BoundingBox(64f, 32f, 32f, 32f),
                new BoundingBox(96f, 32f, 32f, 32f),
            };

        // TODO: Should be fetched from tile file
        public BoundingBox[] StationarySourceRects => new BoundingBox[]
            {
                new BoundingBox(0f, 96f, 32f, 32f)
            };
    }
}
