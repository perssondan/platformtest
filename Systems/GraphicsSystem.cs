using GamesLibrary.Models;
using GamesLibrary.Systems;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using uwpPlatformer.Components;
using uwpPlatformer.Events;
using uwpPlatformer.Extensions;
using uwpPlatformer.GameObjects;
using Windows.System;
using Windows.UI;

namespace uwpPlatformer.Systems
{
    public class GraphicsSystem : SystemBase<GraphicsSystem>
    {
        private readonly Dictionary<Color, CanvasSolidColorBrush> _brushes = new Dictionary<Color, CanvasSolidColorBrush>();
        private readonly List<CollisionArgument> _collisionArguments = new List<CollisionArgument>();
        private readonly IEventSystem _eventSystem;
        private Action<CanvasDrawingSession, GameObject> _drawPositionHistory;
        private Action<CanvasDrawingSession, GameObject> _drawColliderBoundingBox;
        private Action<CanvasDrawingSession, GameObject> _drawVelocityVector;
        private Action<CanvasDrawingSession, GameObject> _drawObjectId;
        private Action<CanvasDrawingSession, GameObject> _drawObjectPositionText;
        private Action<CanvasDrawingSession, TimeSpan, ShapeGraphicsComponent> _drawShapeComponent;
        private Action<CanvasDrawingSession, TimeSpan, AnimatedGraphicsComponent> _drawComponent;
        private Action<CanvasDrawingSession, CollisionArgument[]> _drawCollisionInfos;

        public GraphicsSystem(IEventSystem eventSystem)
        {
            _drawShapeComponent = DrawShapeComponent;
            _drawComponent = DrawAnimatedComponent;

            _eventSystem = eventSystem;

            _eventSystem.Subscribe<CollisionArgument>(this, (sender, collision) =>
            {
                _collisionArguments.Add(collision);
            });

            _eventSystem.Subscribe<UserInputInfo>(this, (sender, userInputInfo) =>
            {
                switch (userInputInfo.VirtualKey)
                {
                    case VirtualKey.Number1 when userInputInfo.IsPressed:
                        _drawPositionHistory = _drawPositionHistory == null ? DrawPositionHistory : default(Action<CanvasDrawingSession, GameObject>);
                        break;

                    case VirtualKey.Number2 when userInputInfo.IsPressed:
                        _drawComponent = _drawComponent == null ? DrawAnimatedComponent : default(Action<CanvasDrawingSession, TimeSpan, AnimatedGraphicsComponent>);
                        _drawShapeComponent = _drawShapeComponent == null ? DrawShapeComponent : default(Action<CanvasDrawingSession, TimeSpan, ShapeGraphicsComponent>);
                        break;

                    case VirtualKey.Number3 when userInputInfo.IsPressed:
                        _drawColliderBoundingBox = _drawColliderBoundingBox == null ? DrawColliderBoundingBox : default(Action<CanvasDrawingSession, GameObject>);
                        break;

                    case VirtualKey.Number4 when userInputInfo.IsPressed:
                        _drawCollisionInfos = _drawCollisionInfos == null ? DrawCollisionArguments : default(Action<CanvasDrawingSession, CollisionArgument[]>);
                        break;

                    case VirtualKey.Number5 when userInputInfo.IsPressed:
                        _drawVelocityVector = _drawVelocityVector == null ? DrawVelocityVector : default(Action<CanvasDrawingSession, GameObject>);
                        break;

                    case VirtualKey.Number6 when userInputInfo.IsPressed:
                        _drawObjectId = _drawObjectId == null ? DrawObjectId : default(Action<CanvasDrawingSession, GameObject>);
                        break;

                    case VirtualKey.Number7 when userInputInfo.IsPressed:
                        _drawObjectPositionText = _drawObjectPositionText == null ? DrawObjectPositionText : default(Action<CanvasDrawingSession, GameObject>);
                        break;
                }
            });
        }

        public override void Update(TimingInfo timingInfo)
        {
        }

        public void Draw(CanvasDrawingSession canvasDrawingSession, TimeSpan deltaTime)
        {
            GraphicsComponentManager.Instance.Components
                .OfType<AnimatedGraphicsComponent>()
                .ForEach(graphicsComponent => _drawComponent?.Invoke(canvasDrawingSession, deltaTime, graphicsComponent));

            ShapeGraphicsComponentManager.Instance.Components
                .ForEach(shapeGraphicsComponent => _drawShapeComponent?.Invoke(canvasDrawingSession, deltaTime, shapeGraphicsComponent));

            GameObjectManager.GameObjects
                .ForEach(gameObject =>
                {
                    _drawPositionHistory?.Invoke(canvasDrawingSession, gameObject);
                    _drawColliderBoundingBox?.Invoke(canvasDrawingSession, gameObject);
                    _drawVelocityVector?.Invoke(canvasDrawingSession, gameObject);
                    _drawObjectId?.Invoke(canvasDrawingSession, gameObject);
                    _drawObjectPositionText?.Invoke(canvasDrawingSession, gameObject);
                });

            _drawCollisionInfos?.Invoke(canvasDrawingSession, _collisionArguments.ToArray());

            _collisionArguments.Clear();
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

        private void DrawAnimatedComponent(CanvasDrawingSession canvasDrawingSession, TimeSpan deltaTime, AnimatedGraphicsComponent animatedGraphicsComponent)
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
        }

        private void DrawObjectId(CanvasDrawingSession canvasDrawingSession, GameObject gameObject)
        {
            canvasDrawingSession.DrawText($"{gameObject.Id}",
                                          gameObject.TransformComponent.Position + new Vector2(5f, 17f),
                                          Colors.White,
                                          new CanvasTextFormat { FontSize = 6 });
        }

        private void DrawObjectPositionText(CanvasDrawingSession canvasDrawingSession, GameObject gameObject)
        {
            canvasDrawingSession.DrawText($"{gameObject.TransformComponent.Position}",
                                          gameObject.TransformComponent.Position + new Vector2(0f, 10f),
                                          Colors.White,
                                          new CanvasTextFormat { FontSize = 6 });
        }

        private void DrawColliderBoundingBox(CanvasDrawingSession canvasDrawingSession, GameObject gameObject)
        {
            var colliderComponent = gameObject?.ColliderComponent;
            if (colliderComponent == null) return;

            canvasDrawingSession.DrawRectangle(colliderComponent.BoundingBox, GetCachedBrush(canvasDrawingSession, Colors.LightBlue));
            canvasDrawingSession.FillCircle(colliderComponent.Center, 1.5f, GetCachedBrush(canvasDrawingSession, Colors.LightBlue));
            canvasDrawingSession.FillCircle(colliderComponent.BoundingBox.TopLeft(), 1.5f, GetCachedBrush(canvasDrawingSession, Colors.LightBlue));
            canvasDrawingSession.FillCircle(colliderComponent.BoundingBox.TopRight(), 1.5f, GetCachedBrush(canvasDrawingSession, Colors.LightBlue));
            canvasDrawingSession.FillCircle(colliderComponent.BoundingBox.BottomLeft(), 1.5f, GetCachedBrush(canvasDrawingSession, Colors.LightBlue));
            canvasDrawingSession.FillCircle(colliderComponent.BoundingBox.BottomRight(), 1.5f, GetCachedBrush(canvasDrawingSession, Colors.LightBlue));
        }

        private void DrawVelocityVector(CanvasDrawingSession canvasDrawingSession, GameObject gameObject)
        {
            if (gameObject.TransformComponent.Velocity.LengthSquared() > 0f)
            {
                var center = gameObject.ColliderComponent?.Center ?? gameObject.TransformComponent.Position;
                canvasDrawingSession.DrawLine(center,
                                              center + (gameObject.TransformComponent.Velocity.Normalize() * 15f),
                                              GetCachedBrush(canvasDrawingSession, Colors.Aquamarine));
            }
        }

        private void DrawCollisionArguments(CanvasDrawingSession canvasDrawingSession, CollisionArgument[] collisionArguments)
        {
            var collisionColor = Colors.Orange;
            foreach (var collisionArgument in collisionArguments)
            {
                if (collisionArgument.GameObject.ColliderComponent.CollisionType == ColliderComponent.CollisionTypes.Dynamic)
                {
                    var startPoint = collisionArgument.CollisionInfo.CollisionPoint - (collisionArgument.CollisionInfo.CollisionNormal * collisionArgument.GameObject.ColliderComponent.Size / 2f);
                    // draw collision point
                    canvasDrawingSession.FillCircle(startPoint,
                                                    2f,
                                                    GetCachedBrush(canvasDrawingSession, collisionColor));

                    // draw collision normal

                    canvasDrawingSession.DrawLine(startPoint,
                                                  startPoint + (collisionArgument.CollisionInfo.CollisionNormal * 16f),
                                                  GetCachedBrush(canvasDrawingSession, collisionColor));
                }

                // draw collision rectangle
                canvasDrawingSession.DrawRectangle(collisionArgument.CollisionInfo.ContactRect, GetCachedBrush(canvasDrawingSession, collisionColor));
            }
        }

        private void DrawPositionHistory(CanvasDrawingSession canvasDrawingSession, GameObject gameObject)
        {
            if (gameObject.TransformComponent.PositionHistory.Length < 5) return;

            var canvasPathBuilder = new CanvasPathBuilder(canvasDrawingSession);

            var center = gameObject.ColliderComponent?.Center ?? gameObject.TransformComponent.Position;
            var offset = center - gameObject.TransformComponent.Position;

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
