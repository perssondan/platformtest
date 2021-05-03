using Microsoft.Graphics.Canvas;
using System;
using uwpKarate.GameObjects;
using Windows.Foundation;

namespace uwpKarate.Components
{
    public class GraphicsComponent : GraphicsComponentBase
    {
        private readonly CanvasBitmap _canvasBitmap;
        private readonly Rect _tileSourceRect;

        public GraphicsComponent(GameObject gameObject,
                                 CanvasBitmap canvasBitmap,
                                 int horizontalTileOffset,
                                 int verticalTileOffset,
                                 int tileWidth = 32,
                                 int tileHeight = 32)
            : base(gameObject)
        {
            _canvasBitmap = canvasBitmap;
            _tileSourceRect = new Rect(horizontalTileOffset, verticalTileOffset, tileWidth, tileHeight);
        }

        public override void OnUpdate(CanvasDrawingSession canvasDrawingSession, TimeSpan timeSpan)
        {
            canvasDrawingSession.DrawImage(_canvasBitmap,
                                           GameObject.TransformComponent.Position,
                                           _tileSourceRect);
        }
    }
}