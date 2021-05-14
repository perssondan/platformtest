using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using uwpKarate.Components;
using uwpKarate.Extensions;
using uwpKarate.GameObjects;
using Windows.UI;

namespace uwpKarate.Systems
{
    public class GraphicsSystem : SystemBase<GraphicsSystem>
    {
        private readonly Dictionary<Color, CanvasSolidColorBrush> _brushes = new Dictionary<Color, CanvasSolidColorBrush>();

        public override void Update(World world, TimeSpan deltaTime)
        {
        }

        public void Draw(CanvasDrawingSession canvasDrawingSession, TimeSpan deltaTime)
        {
            GraphicsComponentManager.Instance.Components
                .OfType<AnimatedGraphicsComponent>()
                .ForEach(graphicsComponent => DrawComponent(canvasDrawingSession, deltaTime, graphicsComponent));

            ShapeGraphicsComponentManager.Instance.Components
                .ForEach(shapeGraphicsComponent => DrawShapeComponent(canvasDrawingSession, deltaTime, shapeGraphicsComponent));
        }

        private void DrawShapeComponent(CanvasDrawingSession canvasDrawingSession, TimeSpan deltaTime, ShapeGraphicsComponent shapeGraphicsComponent)
        {
            var position = shapeGraphicsComponent.GameObject.TransformComponent.Position;
            switch (shapeGraphicsComponent.ShapeType)
            {
                case ShapeType.None:
                    break;
                case ShapeType.Rectangle:
                    canvasDrawingSession.FillRectangle(position.X, position.Y, shapeGraphicsComponent.Size.X, shapeGraphicsComponent.Size.Y, GetCachedBrush(canvasDrawingSession, shapeGraphicsComponent.Color));
                    break;
                case ShapeType.Ellipse:
                    canvasDrawingSession.FillEllipse(position, shapeGraphicsComponent.Size.X / 2f, shapeGraphicsComponent.Size.Y / 2f, GetCachedBrush(canvasDrawingSession, shapeGraphicsComponent.Color));
                    break;
                case ShapeType.Square:
                    var size = Math.Max(shapeGraphicsComponent.Size.X, shapeGraphicsComponent.Size.Y);
                    canvasDrawingSession.FillRectangle(position.X, position.Y, size, size, GetCachedBrush(canvasDrawingSession, shapeGraphicsComponent.Color));
                    break;
                case ShapeType.Circle:
                    var radius = Math.Max(shapeGraphicsComponent.Size.X, shapeGraphicsComponent.Size.Y) / 2f;
                    canvasDrawingSession.FillCircle(position, radius, GetCachedBrush(canvasDrawingSession, shapeGraphicsComponent.Color));
                    break;
            }
        }

        private void DrawParticle(CanvasDrawingSession canvasDrawingSession, TimeSpan deltaTime, ParticleComponent particleComponent)
        {
            canvasDrawingSession.FillCircle(particleComponent.GameObject.TransformComponent.Position,
                                                        1.5f, GetCachedBrush(canvasDrawingSession, Colors.White));
        }

        private void DrawComponent(CanvasDrawingSession canvasDrawingSession, TimeSpan deltaTime, AnimatedGraphicsComponent animatedGraphicsComponent)
        {
            var sourceRects = animatedGraphicsComponent.SourceRects;
            var numberOfTiles = sourceRects.Count();

            if (numberOfTiles <= 0) return;

            if (animatedGraphicsComponent.CurrentTileIndex >= numberOfTiles)
            {
                animatedGraphicsComponent.CurrentTileIndex = 0;
            }

            var position = animatedGraphicsComponent.GameObject.TransformComponent.Position;
            var currentSourceRect = sourceRects[animatedGraphicsComponent.CurrentTileIndex];
            if (animatedGraphicsComponent.InvertTile)
            {
                var invertedCenter = new Vector3(
                    position.X + (float)currentSourceRect.Width / 2f,
                    position.Y,
                    0f);
                var invertMatrix = Matrix4x4.CreateScale(-1f,
                                                         1f,
                                                         0f,
                                                         invertedCenter);
                canvasDrawingSession.DrawImage(animatedGraphicsComponent.CanvasBitmap,
                                               position,
                                               currentSourceRect,
                                               1f,
                                               CanvasImageInterpolation.NearestNeighbor,
                                               invertMatrix);
            }
            else
            {
                canvasDrawingSession.DrawImage(animatedGraphicsComponent.CanvasBitmap,
                                               position,
                                               currentSourceRect);
            }

            animatedGraphicsComponent.CurrentTime += deltaTime;
            if (animatedGraphicsComponent.CurrentTime >= animatedGraphicsComponent.AnimationInterval)
            {
                animatedGraphicsComponent.CurrentTime = TimeSpan.Zero;
                animatedGraphicsComponent.CurrentTileIndex++;
            }

            #region Debugging stuff

            DrawPositionHistory(canvasDrawingSession, animatedGraphicsComponent.GameObject);
            DrawRectangleIfColliding(canvasDrawingSession, animatedGraphicsComponent.GameObject);
            DrawColliderBoundingBox(canvasDrawingSession, animatedGraphicsComponent.GameObject.ColliderComponent);
            DrawCollisionPoint(canvasDrawingSession, animatedGraphicsComponent.GameObject.ColliderComponent);
            DrawVelocityVector(canvasDrawingSession, animatedGraphicsComponent.GameObject);

            #endregion Debugging stuff
        }

        protected void DrawColliderBoundingBox(CanvasDrawingSession canvasDrawingSession, ColliderComponent colliderComponent)
        {
            if (colliderComponent == null) return;

            canvasDrawingSession.DrawRectangle(colliderComponent.BoundingBox, GetCachedBrush(canvasDrawingSession, Colors.Fuchsia));
        }

        protected void DrawVelocityVector(CanvasDrawingSession canvasDrawingSession, GameObject gameObject)
        {
            if (gameObject.TransformComponent.Velocity.LengthSquared() > 0f)
            {
                canvasDrawingSession.DrawLine(gameObject.ColliderComponent.Center,
                                              gameObject.ColliderComponent.Center + (gameObject.TransformComponent.Velocity.Normalize() * 20f),
                                              GetCachedBrush(canvasDrawingSession, Colors.Aquamarine));
            }
        }

        protected void DrawCollisionPoint(CanvasDrawingSession canvasDrawingSession, ColliderComponent colliderComponent)
        {
            var collisionInfos = colliderComponent?.CollisionInfos;
            if (collisionInfos == null) return;

            foreach (var collisionInfo in collisionInfos)
            {
                canvasDrawingSession.FillCircle(collisionInfo.CollisionPoint,
                                                        3f, GetCachedBrush(canvasDrawingSession, Colors.Orange));

                canvasDrawingSession.DrawRectangle(collisionInfo.ContactRect, GetCachedBrush(canvasDrawingSession, Colors.Orange));

                var startPoint = collisionInfo.CollisionPoint - (collisionInfo.CollisionNormal * colliderComponent.Size / 2f);
                canvasDrawingSession.DrawLine(startPoint,
                                              startPoint - (collisionInfo.CollisionNormal * 15f),
                                              GetCachedBrush(canvasDrawingSession, Colors.Orange));
            }
        }

        protected void DrawRectangleIfColliding(CanvasDrawingSession canvasDrawingSession, GameObject gameObject)
        {
            if (gameObject?.ColliderComponent?.IsColliding != true) return;

            canvasDrawingSession.DrawRectangle(gameObject.ColliderComponent.BoundingBox, GetCachedBrush(canvasDrawingSession, gameObject.InputComponent != null ? Colors.Yellow : Colors.Blue));
        }

        protected void DrawPositionHistory(CanvasDrawingSession canvasDrawingSession, GameObject gameObject)
        {
            if (gameObject.TransformComponent.PositionHistory.Length < 5) return;

            var canvasPathBuilder = new CanvasPathBuilder(canvasDrawingSession);

            canvasPathBuilder.BeginFigure(gameObject.TransformComponent.PositionHistory.First());
            gameObject.TransformComponent.PositionHistory.Skip(1).All(vector =>
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
