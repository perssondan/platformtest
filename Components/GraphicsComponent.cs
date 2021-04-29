using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using uwpKarate.GameObjects;
using Windows.Foundation;
using Windows.UI;

namespace uwpKarate.Components
{
    public class GraphicsComponent : GameObjectComponent, IGameObjectComponent<CanvasDrawingSession>
    {
        private readonly CanvasBitmap _canvasBitmap;
        private readonly Rect _tileSourceRect;
        private readonly Dictionary<Color, CanvasSolidColorBrush> _brushes = new Dictionary<Color, CanvasSolidColorBrush>();

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

        public void Update(CanvasDrawingSession canvasDrawingSession, TimeSpan timeSpan)
        {
            canvasDrawingSession.DrawImage(_canvasBitmap,
                                           GameObject.TransformComponent.Position,
                                           _tileSourceRect);

            #region Debugging stuff

            DrawPositionHistory(canvasDrawingSession);
            DrawCollision(canvasDrawingSession);

            #endregion Debugging stuff
        }

        private void DrawCollision(CanvasDrawingSession canvasDrawingSession)
        {
            if (GameObject?.ColliderComponent?.IsColliding != true) return;

            canvasDrawingSession.DrawRectangle(GameObject.ColliderComponent.Rect, GetCachedBrush(canvasDrawingSession, GameObject.InputComponent != null ? Colors.Yellow : Colors.Blue));
        }

        private void DrawPositionHistory(CanvasDrawingSession canvasDrawingSession)
        {
            if (GameObject.TransformComponent.PositionHistory.Length < 5) return;

            var canvasPathBuilder = new CanvasPathBuilder(canvasDrawingSession);

            canvasPathBuilder.BeginFigure(GameObject.TransformComponent.PositionHistory.First());
            GameObject.TransformComponent.PositionHistory.Skip(1).All(vector =>
            {
                canvasPathBuilder.AddLine(vector);
                return true;
            });

            canvasPathBuilder.EndFigure(CanvasFigureLoop.Open);
            var pathGeometry = CanvasGeometry.CreatePath(canvasPathBuilder);
            canvasDrawingSession.DrawGeometry(pathGeometry, GetCachedBrush(canvasDrawingSession, Colors.Red));
        }

        private ICanvasBrush GetCachedBrush(ICanvasResourceCreator canvasResourceCreator, Color colors)
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