using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Numerics;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Components
{
    public class AnimatedGraphicsComponent : ComponentBase, IComponent
    {
        public static readonly Vector2 DefaultSpriteSize = new Vector2(32f, 32f);

        public AnimatedGraphicsComponent(GameObject gameObject,
                                 CanvasBitmap canvasBitmap,
                                 IReadOnlyList<int> sourceSpriteIndexes,
                                 TimeSpan animationInterval,
                                 int spriteMapColumns = 4,
                                 Vector2? spriteSize = null)
            : base(gameObject)
        {
            CanvasBitmap = canvasBitmap;
            SourceSpriteIndexes = sourceSpriteIndexes;
            AnimationInterval = animationInterval;
            SpriteMapColumns = spriteMapColumns;
            SpriteSize = spriteSize ?? DefaultSpriteSize;
        }

        /// <summary>
        /// Sprite size. Width and Height in pixels of the sprite.
        /// </summary>
        public Vector2 SpriteSize { get; set; } = DefaultSpriteSize;

        /// <summary>
        /// Number of columns in the sprite map.
        /// </summary>
        public int SpriteMapColumns { get; set; } = 4;

        /// <summary>
        /// Animation interval. Delay between change of sprite.
        /// </summary>
        public TimeSpan AnimationInterval { get; set; }

        /// <summary>
        /// Current sprite animation index.
        /// </summary>
        public int CurrentSpriteIndex { get; set; }

        /// <summary>
        /// Invert tile.
        /// </summary>
        public bool InvertTile { get; set; }

        /// <summary>
        /// Indexes of the sprites in the sprite map.
        /// </summary>
        public IReadOnlyList<int> SourceSpriteIndexes { get; set; } = Array.Empty<int>();

        /// <summary>
        /// Source sprite map.
        /// </summary>
        public CanvasBitmap CanvasBitmap { get; private set; }

        /// <summary>
        /// Keeps the current time of the animation.
        /// </summary>
        public TimeSpan CurrentTime { get; set; }

        /// <summary>
        /// If the component should be rendered or not.
        /// </summary>
        public bool IsVisible { get; set; } = true;
    }
}
