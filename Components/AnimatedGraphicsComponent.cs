using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using uwpPlatformer.GameObjects;
using uwpPlatformer.Numerics;

namespace uwpPlatformer.Components
{
    public class AnimatedGraphicsComponent : ComponentBase, IComponent
    {
        public AnimatedGraphicsComponent(GameObject gameObject,
                                 CanvasBitmap canvasBitmap,
                                 IReadOnlyList<BoundingBox> sourceRects,
                                 TimeSpan animationInterval)
            : base(gameObject)
        {
            CanvasBitmap = canvasBitmap;
            SourceRects = sourceRects;
            AnimationInterval = animationInterval;
        }

        public int NumberOfTiles => SourceRects?.Count() ?? 0;
        public int HorizontalTileOffset { get; set; }
        public int VerticalTileOffset { get; set; }
        public TimeSpan AnimationInterval { get; set; }
        public int CurrentTileIndex { get; set; } = 0;
        public bool InvertTile { get; set; }
        public IReadOnlyList<BoundingBox> SourceRects { get; set; }

        public CanvasBitmap CanvasBitmap { get; private set; }

        public TimeSpan CurrentTime { get; set; }

        public bool IsVisible { get; set; } = true;
    }
}
