using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using uwpKarate.Extensions;
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
        private Dictionary<Color, CanvasSolidColorBrush> _brushes = new Dictionary<Color, CanvasSolidColorBrush>();

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
            canvasDrawingSession.DrawGeometry(pathGeometry, GetBrush(canvasDrawingSession, Colors.Red));

            if (_gameObject?.ColliderComponent?.IsColliding == true)
            {
                canvasDrawingSession.DrawRectangle(_gameObject.TransformComponent.Position.ToRect(32f, 32f), GetBrush(canvasDrawingSession, Colors.Blue));
            }
        }

        private ICanvasBrush GetBrush(ICanvasResourceCreator canvasResourceCreator, Color colors)
        {
            if (_brushes.TryGetValue(colors, out var brush))
            {
                return brush;
            }

            brush = new CanvasSolidColorBrush(canvasResourceCreator, colors);

            _brushes[colors] = brush;

            return brush;
        }
    }
}