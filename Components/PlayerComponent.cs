using System;
using uwpPlatformer.GameObjects;

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

        public static int[] WalkSourceSpriteIndexes => new int[]
            {
                0,
                1,
                2,
                3,
            };

        public static int[] StationarySourceSpriteIndexes => new int[]
            {
                8
            };
    }
}
