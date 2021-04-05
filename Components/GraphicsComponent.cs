using Microsoft.Graphics.Canvas;
using System;
using System.Numerics;
using uwpKarate.Actors;
using Windows.Foundation;

namespace uwpKarate.Models
{
    public class GraphicsComponent
    {
        private readonly CanvasBitmap _canvasBitmap;
        private readonly int _horizontalTileOffset;
        private readonly int _verticalTileOffset;

        public GraphicsComponent(CanvasBitmap canvasBitmap, int horizontalTileOffset, int verticalTileOffset)
        {
            _canvasBitmap = canvasBitmap;
            _horizontalTileOffset = horizontalTileOffset;
            _verticalTileOffset = verticalTileOffset;
        }

        public CanvasBitmap CanvasBitmap => _canvasBitmap;

        public void Draw(CanvasDrawingSession canvasDrawingSession, GameObject gameObject)
        {
            canvasDrawingSession.DrawImage(CanvasBitmap,
                                           new Vector2(gameObject.XPos, gameObject.YPos),
                                           new Rect(_horizontalTileOffset, _verticalTileOffset, 32, 32));
        }
    }
}