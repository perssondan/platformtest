using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using uwpKarate.Extensions;
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
            //DrawCollision(canvasDrawingSession);
            //DrawIsGrounded(canvasDrawingSession);
            DrawCollisionPoint(canvasDrawingSession);
            DrawVelocityVector(canvasDrawingSession);

            #endregion Debugging stuff
        }

        public abstract void OnUpdate(CanvasDrawingSession target, TimeSpan timeSpan);

        protected void DrawIsGrounded(CanvasDrawingSession canvasDrawingSession)
        {
            if (GameObject.InputComponent == null) return;
            if (GameObject?.ColliderComponent.IsColliding != true) return;

            canvasDrawingSession.DrawRectangle(new Rect(GameObject.ColliderComponent.BoundingBox.Left,
                                                        GameObject.ColliderComponent.BoundingBox.Bottom,
                                                        GameObject.ColliderComponent.BoundingBox.Width,
                                                        1f), GetCachedBrush(canvasDrawingSession, Colors.Fuchsia));
        }

        protected void DrawVelocityVector(CanvasDrawingSession canvasDrawingSession)
        {
            if (GameObject.TransformComponent.Velocity.LengthSquared() > 0f)
            {
                canvasDrawingSession.DrawLine(GameObject.ColliderComponent.Center, GameObject.ColliderComponent.Center + (GameObject.TransformComponent.Velocity.Normalize() * 20f), GetCachedBrush(canvasDrawingSession, Colors.Aquamarine));
            }
        }

        protected void DrawCollisionPoint(CanvasDrawingSession canvasDrawingSession)
        {
            var collisionInfos = GameObject?.ColliderComponent?.CollisionInfos;
            if (collisionInfos == null) return;

            foreach(var collisionInfo in collisionInfos)
            {
                canvasDrawingSession.FillCircle(collisionInfo.CollisionPoint,
                                                        5f, GetCachedBrush(canvasDrawingSession, Colors.Yellow));

                canvasDrawingSession.DrawRectangle(collisionInfo.ContactRect, GetCachedBrush(canvasDrawingSession, Colors.Yellow));
            }
        }

        protected void DrawCollision(CanvasDrawingSession canvasDrawingSession)
        {
            if (GameObject?.ColliderComponent?.IsColliding != true) return;

            canvasDrawingSession.DrawRectangle(GameObject.ColliderComponent.BoundingBox, GetCachedBrush(canvasDrawingSession, GameObject.InputComponent != null ? Colors.Yellow : Colors.Blue));
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