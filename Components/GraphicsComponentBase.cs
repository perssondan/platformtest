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
    public abstract class GraphicsComponentBase
        : GameObjectComponent, IGameObjectComponent<CanvasDrawingSession>
    {
        private readonly Dictionary<Color, CanvasSolidColorBrush> _brushes = new Dictionary<Color, CanvasSolidColorBrush>();

        public GraphicsComponentBase(GameObject gameObject)
            : base(gameObject)
        {
        }

        public bool InvertTile { get; set; }
        public IReadOnlyList<Rect> SourceRects { get; set; }

        public void Update(CanvasDrawingSession canvasDrawingSession, TimeSpan timeSpan)
        {
            OnUpdate(canvasDrawingSession, timeSpan);

            #region Debugging stuff

            DrawPositionHistory(canvasDrawingSession);
            DrawCollision(canvasDrawingSession);
            DrawIsGrounded(canvasDrawingSession);

            #endregion Debugging stuff
        }

        public abstract void OnUpdate(CanvasDrawingSession target, TimeSpan timeSpan);

        protected void DrawIsGrounded(CanvasDrawingSession canvasDrawingSession)
        {
            if (GameObject.InputComponent == null) return;
            if (GameObject?.ColliderComponent?.IsGrounded() != true) return;

            canvasDrawingSession.DrawRectangle(new Rect(GameObject.ColliderComponent.Rect.Left,
                                                        GameObject.ColliderComponent.Rect.Bottom,
                                                        GameObject.ColliderComponent.Rect.Width,
                                                        1f), GetCachedBrush(canvasDrawingSession, Colors.Fuchsia));
        }

        protected void DrawCollision(CanvasDrawingSession canvasDrawingSession)
        {
            if (GameObject?.ColliderComponent?.IsColliding != true) return;

            canvasDrawingSession.DrawRectangle(GameObject.ColliderComponent.Rect, GetCachedBrush(canvasDrawingSession, GameObject.InputComponent != null ? Colors.Yellow : Colors.Blue));
        }

        protected void DrawPositionHistory(CanvasDrawingSession canvasDrawingSession)
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