using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Linq;
using System.Numerics;
using uwpKarate.GameObjects;
using Windows.Foundation;
using Windows.UI;

namespace uwpKarate.Components
{
    public class GraphicsComponent : IGameObjectComponent<CanvasDrawingSession>
    {
        private readonly GameObject _gameObject;
        private readonly CanvasBitmap _canvasBitmap;
        private readonly Rect _rect;
        private CanvasSolidColorBrush _canvasSolidColorBrush;

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

        public void Update(CanvasDrawingSession canvasDrawingSession, TimeSpan timeSpan)
        {
            canvasDrawingSession.DrawImage(CanvasBitmap,
                                           _gameObject.TransformComponent.Position,
                                           _rect);

            if (_gameObject.TransformComponent.PositionHistory.Length < 5) return;

            var canvasPathBuilder = new CanvasPathBuilder(canvasDrawingSession);

            canvasPathBuilder.BeginFigure(_gameObject.TransformComponent.PositionHistory.First());
            _gameObject.TransformComponent.PositionHistory.Skip(1).All(vector =>
            {
                canvasPathBuilder.AddLine(vector);
                return true;
            });
            canvasPathBuilder.EndFigure(CanvasFigureLoop.Open);
            var pathGeometry = CanvasGeometry.CreatePath(canvasPathBuilder);
            //var geometry = CanvasGeometry.CreatePolygon(canvasDrawingSession, _gameObject.TransformComponent.PositionHistory);
            canvasDrawingSession.DrawGeometry(pathGeometry, GetBrush(canvasDrawingSession));
        }

        private ICanvasBrush GetBrush(ICanvasResourceCreator canvasResourceCreator)
        {
            if (_canvasSolidColorBrush == null)
            {
                _canvasSolidColorBrush = new CanvasSolidColorBrush(canvasResourceCreator, Colors.Red);
            }

            return _canvasSolidColorBrush;
        }
    }
}