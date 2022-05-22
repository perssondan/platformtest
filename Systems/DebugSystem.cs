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
    public class DebugSystem : ISystem
    {
        private readonly Dictionary<Color, CanvasSolidColorBrush> _brushes = new Dictionary<Color, CanvasSolidColorBrush>();
        private readonly List<CollisionEvent> _collisionArguments = new List<CollisionEvent>();
        private readonly IEventSystem _eventSystem;
        private readonly IGameObjectManager _gameObjectManager;
        private Action<CanvasDrawingSession, GameObject> _drawPositionHistory;
        private Action<CanvasDrawingSession, GameObject> _drawColliderBoundingBox;
        private Action<CanvasDrawingSession, GameObject> _drawVelocityVector;
        private Action<CanvasDrawingSession, GameObject> _drawForceVector;
        private Action<CanvasDrawingSession, GameObject> _drawObjectId;
        private Action<CanvasDrawingSession, GameObject> _drawObjectPositionText;
        private Action<CanvasDrawingSession, CollisionEvent[]> _drawCollisionInfos;

        public DebugSystem(IEventSystem eventSystem, IGameObjectManager gameObjectManager)
        {
            _eventSystem = eventSystem;
            _gameObjectManager = gameObjectManager;

            _eventSystem.Subscribe<CollisionEvent>(this, (sender, collision) =>
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
                        ToggleVisibility();
                        break;

                    case VirtualKey.Number3 when userInputInfo.IsPressed:
                        _drawColliderBoundingBox = _drawColliderBoundingBox == null ? DrawColliderBoundingBox : default(Action<CanvasDrawingSession, GameObject>);
                        break;

                    case VirtualKey.Number4 when userInputInfo.IsPressed:
                        _drawCollisionInfos = _drawCollisionInfos == null ? DrawCollisionArguments : default(Action<CanvasDrawingSession, CollisionEvent[]>);
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

                    case VirtualKey.Number8 when userInputInfo.IsPressed:
                        _drawForceVector = _drawForceVector is null ? DrawForceVector : default(Action<CanvasDrawingSession, GameObject>);
                        break;
                }
            });
        }

        public string Name => nameof(DebugSystem);

        public void Update(TimingInfo timingInfo)
        {
        }

        public void Draw(CanvasDrawingSession canvasDrawingSession, TimeSpan deltaTime)
        {
            _gameObjectManager.GameObjects
                .ForEach(gameObject =>
                {
                    _drawPositionHistory?.Invoke(canvasDrawingSession, gameObject);
                    _drawColliderBoundingBox?.Invoke(canvasDrawingSession, gameObject);
                    _drawVelocityVector?.Invoke(canvasDrawingSession, gameObject);
                    _drawObjectId?.Invoke(canvasDrawingSession, gameObject);
                    _drawObjectPositionText?.Invoke(canvasDrawingSession, gameObject);
                    _drawForceVector?.Invoke(canvasDrawingSession, gameObject);
                });

            _drawCollisionInfos?.Invoke(canvasDrawingSession, _collisionArguments.ToArray());

            _collisionArguments.Clear();
        }

        private void ToggleVisibility()
        {
            _gameObjectManager.GameObjects
                .Select(gameObject => gameObject.GetComponent<AnimatedGraphicsComponent>())
                .Where(graphicsComponent => graphicsComponent != default)
                .ForEach(graphicsComponent =>
                {
                    ToggleAnimatedComponentVisibility(graphicsComponent);
                });

            _gameObjectManager.GameObjects
                .Select(gameObject => gameObject.GetComponent<ShapeGraphicsComponent>())
                .Where(shapeGraphicsComponent => shapeGraphicsComponent != default)
                .ForEach(shapeGraphicsComponent =>
                {
                    ToggleShapeComponentVisibility(shapeGraphicsComponent);
                });
        }

        private void ToggleAnimatedComponentVisibility(AnimatedGraphicsComponent animatedGraphicsComponent)
        {
            animatedGraphicsComponent.IsVisible = !animatedGraphicsComponent.IsVisible;
        }

        private void ToggleShapeComponentVisibility(ShapeGraphicsComponent shapeGraphicsComponent)
        {
            shapeGraphicsComponent.IsVisible = !shapeGraphicsComponent.IsVisible;
        }

        private void DrawObjectId(CanvasDrawingSession canvasDrawingSession, GameObject gameObject)
        {
            if (!gameObject.TryGetComponent<TransformComponent>(out var transformComponent)) return;
            canvasDrawingSession.DrawText($"{gameObject.Id}",
                                          transformComponent.Position + new Vector2(5f, 17f),
                                          Colors.White,
                                          new CanvasTextFormat { FontSize = 6 });
        }

        private void DrawObjectPositionText(CanvasDrawingSession canvasDrawingSession, GameObject gameObject)
        {
            if (!gameObject.TryGetComponent<ColliderComponent>(out var colliderComponent)) return;
            if (!gameObject.TryGetComponent<TransformComponent>(out var transformComponent)) return;

            var x = colliderComponent.Center.X;
            var y = colliderComponent.Center.Y;
            canvasDrawingSession.DrawText($"{x:0.##} : {y:0.##}",
                                          transformComponent.Position + new Vector2(0f, 10f),
                                          Colors.LightGray,
                                          new CanvasTextFormat { FontSize = 7 });
        }

        private void DrawColliderBoundingBox(CanvasDrawingSession canvasDrawingSession, GameObject gameObject)
        {
            if (!gameObject.TryGetComponent<ColliderComponent>(out var colliderComponent)) return;

            canvasDrawingSession.DrawRectangle(colliderComponent.BoundingBox, GetCachedBrush(canvasDrawingSession, Colors.LightBlue));
            canvasDrawingSession.FillCircle(colliderComponent.Center, 1.5f, GetCachedBrush(canvasDrawingSession, Colors.LightBlue));
            canvasDrawingSession.FillCircle(colliderComponent.BoundingBox.TopLeft(), 1.5f, GetCachedBrush(canvasDrawingSession, Colors.LightBlue));
            canvasDrawingSession.FillCircle(colliderComponent.BoundingBox.TopRight(), 1.5f, GetCachedBrush(canvasDrawingSession, Colors.LightBlue));
            canvasDrawingSession.FillCircle(colliderComponent.BoundingBox.BottomLeft(), 1.5f, GetCachedBrush(canvasDrawingSession, Colors.LightBlue));
            canvasDrawingSession.FillCircle(colliderComponent.BoundingBox.BottomRight(), 1.5f, GetCachedBrush(canvasDrawingSession, Colors.LightBlue));
        }

        private void DrawVelocityVector(CanvasDrawingSession canvasDrawingSession, GameObject gameObject)
        {
            if (!gameObject.TryGetComponent<PhysicsComponent>(out var physicsComponent)) return;
            if (!gameObject.TryGetComponent<ColliderComponent>(out var colliderComponent)) return;

            canvasDrawingSession.DrawText($"Velocity <{physicsComponent.Velocity.X}, {physicsComponent.Velocity.Y}>",
                                          new Vector2(375, 15),
                                          Colors.White,
                                          new CanvasTextFormat { FontSize = 8 });

            var velocity = physicsComponent.Velocity;
            if (velocity.LengthSquared() > 0f)
            {
                var center = colliderComponent.Center;
                canvasDrawingSession.DrawLine(center,
                                              center + (velocity.Normalize() * 15f),
                                              GetCachedBrush(canvasDrawingSession, Colors.Aquamarine));
            }
        }

        private void DrawForceVector(CanvasDrawingSession canvasDrawingSession, GameObject gameObject)
        {
            if (!gameObject.TryGetComponent<PhysicsComponent>(out var physicsComponent)) return;
            if (!gameObject.TryGetComponent<ColliderComponent>(out var colliderComponent)) return;

            var lengthSquared = physicsComponent.Acceleration.LengthSquared();

            if (lengthSquared > 0f)
            {
                var center = colliderComponent.Center;
                canvasDrawingSession.DrawLine(center,
                                              center + (physicsComponent.Acceleration.Normalize() * 15f),
                                              GetCachedBrush(canvasDrawingSession, Colors.Yellow), 1f);
            }
        }

        private void DrawCollisionArguments(CanvasDrawingSession canvasDrawingSession, CollisionEvent[] collisionArguments)
        {
            var collisionColor = Colors.Orange;
            foreach (var collisionArgument in collisionArguments)
            {
                if (!collisionArgument.GameObject.TryGetComponent<ColliderComponent>(out var colliderComponent)) continue;

                if ((colliderComponent.CollisionType & ColliderComponent.CollisionTypes.IsDynamicMask) > 0)
                {
                    var startPoint = collisionArgument.CollisionManifold.CollisionPoint - (collisionArgument.CollisionManifold.CollisionNormal * colliderComponent.Size * .5f);
                    // draw collision point
                    canvasDrawingSession.FillCircle(startPoint,
                                                    2f,
                                                    GetCachedBrush(canvasDrawingSession, collisionColor));

                    // draw collision normal

                    canvasDrawingSession.DrawLine(startPoint,
                                                  startPoint + (collisionArgument.CollisionManifold.CollisionNormal * 16f),
                                                  GetCachedBrush(canvasDrawingSession, collisionColor));
                }

                // draw collision rectangle
                canvasDrawingSession.DrawRectangle(collisionArgument.CollisionManifold.ContactRect, GetCachedBrush(canvasDrawingSession, collisionColor));
            }
        }

        private void DrawPositionHistory(CanvasDrawingSession canvasDrawingSession, GameObject gameObject)
        {
            canvasDrawingSession.DrawText("Position history on",
                                          new Vector2(375, 5),
                                          Colors.White,
                                          new CanvasTextFormat { FontSize = 8 });

            var transformComponent = gameObject.GetComponent<TransformComponent>();

            if (transformComponent.PositionHistory.Length < 5) return;

            var canvasPathBuilder = new CanvasPathBuilder(canvasDrawingSession);

            var center = Vector2.Zero;
            if (gameObject.TryGetComponent<ColliderComponent>(out var colliderComponent))
            {
                center = colliderComponent.Center;
            }
            else
            {
                center = transformComponent.Position;
            }

            var offset = center - transformComponent.Position;

            canvasPathBuilder.BeginFigure(transformComponent.PositionHistory.First() + offset);
            transformComponent.PositionHistory.Skip(1).All(vector =>
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
