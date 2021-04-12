﻿using Microsoft.Graphics.Canvas;
using uwpKarate.GameObjects;
using Windows.Foundation;

namespace uwpKarate.Components
{
    public class GraphicsComponent
    {
        private readonly GameObject _gameObject;
        private readonly CanvasBitmap _canvasBitmap;
        private readonly Rect _rect;

        public GraphicsComponent(GameObject gameObject,
                                 CanvasBitmap canvasBitmap,
                                 int horizontalTileOffset,
                                 int verticalTileOffset,
                                 int tileWidth = 32,
                                 int tileHeight = 32)
        {
            _gameObject = gameObject;
            _canvasBitmap = canvasBitmap;
            _rect = new Rect(horizontalTileOffset, verticalTileOffset, tileWidth, tileHeight);
        }

        public CanvasBitmap CanvasBitmap => _canvasBitmap;
        public Rect Rect => _rect;

        public void Draw(CanvasDrawingSession canvasDrawingSession)
        {
            canvasDrawingSession.DrawImage(CanvasBitmap,
                                           _gameObject.TransformComponent.Position,
                                           _rect);
        }
    }
}