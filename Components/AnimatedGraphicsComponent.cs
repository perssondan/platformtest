using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using uwpKarate.GameObjects;
using Windows.Foundation;

namespace uwpKarate.Components
{
    public class AnimatedGraphicsComponent : GraphicsComponentBase
    {
        private CanvasBitmap _canvasBitmap;
        private int _currentTileIndex = 0;
        private TimeSpan _currentTime;

        public AnimatedGraphicsComponent(GameObject gameObject,
                                 CanvasBitmap canvasBitmap,
                                 IReadOnlyList<Rect> sourceRects,
                                 TimeSpan animationInterval)
            : base(gameObject)
        {
            _canvasBitmap = canvasBitmap;
            SourceRects = sourceRects;
            AnimationInterval = animationInterval;
        }

        public int NumberOfTiles => SourceRects?.Count() ?? 0;
        public int HorizontalTileOffset { get; set; }
        public int VerticalTileOffset { get; set; }
        public TimeSpan AnimationInterval { get; set; }

        public override void OnUpdate(CanvasDrawingSession canvasDrawingSession, TimeSpan timeSpan)
        {
            var sourceRects = SourceRects;
            var numberOfTiles = sourceRects.Count();

            if (numberOfTiles <= 0) return;

            if (_currentTileIndex >= numberOfTiles)
            {
                _currentTileIndex = 0;
            }

            if (InvertTile)
            {
                var currentSourceRect = sourceRects[_currentTileIndex];
                var invertedCenter = new Vector3(
                    GameObject.TransformComponent.Position.X + (float)currentSourceRect.Width / 2f,
                    GameObject.TransformComponent.Position.Y,
                    0f);
                var invertMatrix = Matrix4x4.CreateScale(-1f,
                                                         1f,
                                                         0f,
                                                         invertedCenter);
                canvasDrawingSession.DrawImage(_canvasBitmap,
                                           GameObject.TransformComponent.Position,
                                           sourceRects[_currentTileIndex],
                                           1f,
                                           CanvasImageInterpolation.NearestNeighbor,
                                           invertMatrix);
            }
            else
            {
                canvasDrawingSession.DrawImage(_canvasBitmap,
                                           GameObject.TransformComponent.Position,
                                           sourceRects[_currentTileIndex]);
            }

            _currentTime += timeSpan;
            if (_currentTime >= AnimationInterval)
            {
                _currentTime = TimeSpan.Zero;
                _currentTileIndex++;
            }
        }
    }
}