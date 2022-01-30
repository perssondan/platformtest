using System;
using uwpPlatformer.GameObjects;
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
        public TimeSpan WalkPressedAt { get; set; }
        public bool IsJumpButtonPressed { get; set; }

        public Rect[] WalkSourceRects => new Rect[]
            {
                new Rect(0, 32f, 32f, 32f),
                new Rect(32f, 32f, 32f, 32f),
                new Rect(64f, 32f, 32f, 32f),
                new Rect(96f, 32f, 32f, 32f),
            };

        public Rect[] StaticSourceRects => new Rect[]
            {
                new Rect(0f, 96f, 32f, 32f)
            };
    }
}
