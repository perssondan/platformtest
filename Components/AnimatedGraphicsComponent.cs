using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using uwpKarate.GameObjects;
using Windows.Foundation;

namespace uwpKarate.Components
{
    public class AnimatedGraphicsComponent : GameObjectComponent, IComponent
    {
        public AnimatedGraphicsComponent(GameObject gameObject,
                                 CanvasBitmap canvasBitmap,
                                 IReadOnlyList<Rect> sourceRects,
                                 TimeSpan animationInterval)
            : base(gameObject)
        {
            GraphicsComponentManager.Instance.AddComponent(this);
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

        protected override void OnDispose()
        {
            GraphicsComponentManager.Instance.RemoveComponent(this);
        }
    }
}
