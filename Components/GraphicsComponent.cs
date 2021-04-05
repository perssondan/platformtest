using Microsoft.Graphics.Canvas;
using System.Numerics;
using uwpKarate.GameObjects;
using Windows.Foundation;

namespace uwpKarate.Components
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

        public void Update(GameObject gameObject, CanvasDrawingSession canvasDrawingSession)
        {
            canvasDrawingSession.DrawImage(CanvasBitmap,
                                           new Vector2(gameObject.XPos, gameObject.YPos),
                                           new Rect(_horizontalTileOffset, _verticalTileOffset, 32, 32));
        }
    }
}