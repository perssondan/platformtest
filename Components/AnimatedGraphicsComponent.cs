using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using uwpPlatformer.GameObjects;
using Windows.Foundation;

namespace uwpPlatformer.Components
{
    public class AnimatedGraphicsComponent : ComponentBase, IComponent
    {
        public AnimatedGraphicsComponent(GameObject gameObject,
                                 CanvasBitmap canvasBitmap,
                                 IReadOnlyList<Rect> sourceRects,
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
        public IReadOnlyList<Rect> SourceRects { get; set; }

        public CanvasBitmap CanvasBitmap { get; private set; }

        public TimeSpan CurrentTime { get; set; }
    }
}
