﻿using GamesLibrary.Models;
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
    public class GraphicsSystem : ISystem
    {
        private readonly Dictionary<Color, CanvasSolidColorBrush> _brushes = new Dictionary<Color, CanvasSolidColorBrush>();
        private readonly IGameObjectManager _gameObjectManager;
        private Action<CanvasDrawingSession, TimeSpan, ShapeGraphicsComponent> _drawShapeComponent;
        private Action<CanvasDrawingSession, TimeSpan, AnimatedGraphicsComponent> _drawComponent;

        public GraphicsSystem(IGameObjectManager gameObjectManager)
        {
            _drawShapeComponent = DrawShapeComponent;
            _drawComponent = DrawAnimatedComponent;

            _gameObjectManager = gameObjectManager;
        }

        public string Name => nameof(GraphicsSystem);

        public void Update(TimingInfo timingInfo)
        {
        }

        public void Draw(CanvasDrawingSession canvasDrawingSession, TimeSpan deltaTime)
        {
            _gameObjectManager.GameObjects
                .Select(gameObject => gameObject.GetComponent<AnimatedGraphicsComponent>())
                .Where(graphicsComponent => graphicsComponent != default && graphicsComponent.IsVisible)
                .ForEach(graphicsComponent =>
                {
                    _drawComponent?.Invoke(canvasDrawingSession, deltaTime, graphicsComponent);
                });

            _gameObjectManager.GameObjects
                .Select(gameObject => gameObject.GetComponent<ShapeGraphicsComponent>())
                .Where(shapeGraphicsComponent => shapeGraphicsComponent != default && shapeGraphicsComponent.IsVisible)
                .ForEach(shapeGraphicsComponent =>
                {
                    _drawShapeComponent?.Invoke(canvasDrawingSession, deltaTime, shapeGraphicsComponent);
                });
        }

        private void DrawShapeComponent(CanvasDrawingSession canvasDrawingSession, TimeSpan deltaTime, ShapeGraphicsComponent shapeGraphicsComponent)
        {
            if (!shapeGraphicsComponent.GameObject.TryGetComponent<TransformComponent>(out var transformComponent)) return;
            var position = transformComponent.Position;
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

            if (!animatedGraphicsComponent.GameObject.TryGetComponent<TransformComponent>(out var transformComponent)) return;

            var position = transformComponent.Position;
            var currentSourceRect = sourceRects[animatedGraphicsComponent.CurrentTileIndex];
            if (animatedGraphicsComponent.InvertTile)
            {
                var centerPoint = new Vector3(
                    position.X + (float)currentSourceRect.Width / 2f,
                    position.Y,
                    0f);
                var invertMatrix = Matrix4x4.CreateScale(-1f,
                                                         1f,
                                                         0f,
                                                         centerPoint);
                canvasDrawingSession.DrawImage(animatedGraphicsComponent.CanvasBitmap,
                                               position,
                                               currentSourceRect.ToRect(),
                                               1f,
                                               CanvasImageInterpolation.NearestNeighbor,
                                               invertMatrix);
            }
            else
            {
                canvasDrawingSession.DrawImage(animatedGraphicsComponent.CanvasBitmap,
                                               position,
                                               currentSourceRect.ToRect());
            }

            animatedGraphicsComponent.CurrentTime += deltaTime;
            if (animatedGraphicsComponent.CurrentTime >= animatedGraphicsComponent.AnimationInterval)
            {
                animatedGraphicsComponent.CurrentTime = TimeSpan.Zero;
                animatedGraphicsComponent.CurrentTileIndex++;
            }
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
