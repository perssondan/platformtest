using System;
using System.Numerics;
using uwpPlatformer.Constants;
using uwpPlatformer.GameObjects;
using Windows.Foundation;

namespace uwpPlatformer.Components
{
    public class PlayerComponent : GameObjectComponent, IComponent
    {
        public PlayerComponent(GameObject gameObject)
            : base(gameObject)
        {
            PlayerComponentManager.Instance.AddComponent(this);
        }

        public TimeSpan JumpPressedRememberTime => TimeSpan.FromMilliseconds(150);
        public TimeSpan JumpPressedAt { get; set; }
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

        /// <summary>
        /// Initial jump velocity
        /// </summary>
        public Vector2 InitialJumpVelocity { get; set; } = new Vector2(0, PlayerConstants.InitialVerticalVelocity);

        protected override void OnDispose()
        {
            PlayerComponentManager.Instance.RemoveComponent(this);
        }
    }
}
