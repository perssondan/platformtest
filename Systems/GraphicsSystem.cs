using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using uwpKarate.Components;
using uwpKarate.Events;
using uwpKarate.Extensions;
using uwpKarate.GameObjects;
using Windows.UI;

namespace uwpKarate.Systems
{
    public class GraphicsSystem : SystemBase<GraphicsSystem>
    {
        private readonly Dictionary<Color, CanvasSolidColorBrush> _brushes = new Dictionary<Color, CanvasSolidColorBrush>();
        private readonly List<CollisionInfo> _collisionInfos = new List<CollisionInfo>();

        protected override void Initialize()
        {
            EventSystem.Instance.Register<CollisionArgument>(this, collision =>
            {
                _collisionInfos.Add(collision.CollisionInfo);
            });
        }

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

            _collisionInfos.Clear();
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
            //DrawRectangleIfColliding(canvasDrawingSession, animatedGraphicsComponent.GameObject);
            DrawColliderBoundingBox(canvasDrawingSession, animatedGraphicsComponent.GameObject.ColliderComponent);
            //DrawCollisionPoint(canvasDrawingSession, animatedGraphicsComponent.GameObject.ColliderComponent);
            DrawVelocityVector(canvasDrawingSession, animatedGraphicsComponent.GameObject);
            DrawObjectInfo(canvasDrawingSession, animatedGraphicsComponent.GameObject);

            #endregion Debugging stuff
        }

        private void DrawObjectInfo(CanvasDrawingSession canvasDrawingSession, GameObject gameObject)
        {
            canvasDrawingSession.DrawText($"{gameObject.TransformComponent.Position}", gameObject.TransformComponent.Position + new Vector2(0f, 10f), Colors.White, new CanvasTextFormat { FontSize = 6 });
        }

        protected void DrawColliderBoundingBox(CanvasDrawingSession canvasDrawingSession, ColliderComponent colliderComponent)
        {
            if (colliderComponent == null) return;

            canvasDrawingSession.DrawRectangle(colliderComponent.BoundingBox, GetCachedBrush(canvasDrawingSession, Colors.LightBlue));
            canvasDrawingSession.FillCircle(colliderComponent.Center, 1.5f, GetCachedBrush(canvasDrawingSession, Colors.LightBlue));
            //if (colliderComponent.CollisionType == ColliderComponent.CollisionTypes.Static)
            //{
            //    var expandedStaticRect = colliderComponent.BoundingBox.Add(new Vector2(32f, 32f));
            //    canvasDrawingSession.DrawRectangle(expandedStaticRect, GetCachedBrush(canvasDrawingSession, Colors.LightBlue));
            //}

            DrawCollisionInfos(canvasDrawingSession, _collisionInfos.ToArray());
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

            DrawCollisionInfos(canvasDrawingSession, collisionInfos);
        }

        private void DrawCollisionInfos(CanvasDrawingSession canvasDrawingSession, CollisionInfo[] collisionInfos)
        {
            foreach (var collisionInfo in collisionInfos)
            {
                canvasDrawingSession.FillCircle(collisionInfo.CollisionPoint,
                                                        3f, GetCachedBrush(canvasDrawingSession, Colors.Orange));

                canvasDrawingSession.DrawRectangle(collisionInfo.ContactRect, GetCachedBrush(canvasDrawingSession, Colors.Orange));

                var startPoint = collisionInfo.CollisionPoint - (collisionInfo.CollisionNormal * 32f);
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

            var offset = gameObject.ColliderComponent.Center - gameObject.TransformComponent.Position;

            canvasPathBuilder.BeginFigure(gameObject.TransformComponent.PositionHistory.First() + offset);
            gameObject.TransformComponent.PositionHistory.Skip(1).All(vector =>
            {
                canvasPathBuilder.AddLine(vector + offset);
                return true;
            });

            canvasPathBuilder.EndFigure(CanvasFigureLoop.Open);
            var pathGeometry = CanvasGeometry.CreatePath(canvasPathBuilder);
            canvasDrawingSession.DrawGeometry(pathGeometry, GetCachedBrush(canvasDrawingSession, Colors.Red), 1f);
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
