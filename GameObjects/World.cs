using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using uwpKarate.Components;
using uwpKarate.Extensions;
using uwpKarate.Models;
using Windows.Foundation;

namespace uwpKarate.GameObjects
{
    public class World
    {
        private readonly int _mapTileWidth = 14;
        private readonly int _mapTileHeight = 10;
        private const int _tileWidth = 32;
        private const int _tileHeight = 32;
        private readonly IReadOnlyList<CanvasBitmap> _canvasBitmaps;
        private readonly Map _map;
        private readonly TileAtlas[] _tileAtlases;
        private GameObject[] _tiles;

        private List<IGameObjectComponent<CanvasDrawingSession>> _graphicsComponents = new List<IGameObjectComponent<CanvasDrawingSession>>();

        private int[] _mapData = new[]
        {
            3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0,
            3, 0, 0, 0, 3, 0, 0, 0, 0, 0, 3, 0, 0, 0,
            3, 0, 0, 0, 0, 3, 0, 0, 0, 3, 0, 0, 0, 0,
            3, 0, 0, 0, 0, 0, 3, 0, 3, 0, 0, 0, 0, 0,
            3, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            3, 3, 3, 3, 3, 3, 3, 0, 0, 3, 3, 3, 3, 3
        };

        public World(CanvasBitmap[] canvasBitmaps, Map map, TileAtlas[] tileAtlases, Windows.UI.Xaml.Window current)
        {
            _canvasBitmaps = canvasBitmaps;
            _map = map;
            _tileAtlases = tileAtlases;
            _mapData = _map.Layers[0].Data;

            _mapTileWidth = _map.Width;
            _mapTileHeight = _map.Height;
            _tiles = new GameObject[_mapTileWidth * _mapTileHeight];

            InitializeTileMap();
            InitializeHeroine(current);
        }

        public int WorldPixelHeight => _map.Height * _map.TileHeight;
        public int WorldPixelWidth => _map.Width * _map.TileWidth;

        public Rect WorldRect => new Rect(0, 0, WorldPixelWidth, WorldPixelHeight);

        public bool TryGetGroundedTile(GameObject gameObject, out GameObject tileGameObject, float elapsedTimed)
        {
            if (TryGetTileGameObject(gameObject.TransformComponent.Position, out tileGameObject, elapsedTimed))
            {
                if (tileGameObject != null)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsPointInRect(Vector2 point, Rect rect)
        {
            return rect.Contains(point.ToPoint());
        }

        public bool IsRectInRect(Rect firstRect, Rect secondRect)
        {
            return (firstRect.Left < secondRect.Right && firstRect.Right > secondRect.Left &&
                    firstRect.Top < secondRect.Bottom && firstRect.Bottom > secondRect.Top);
        }

        public bool IsRayInRect(Vector2 rayOrigin,
                                Vector2 rayDirection,
                                Rect target,
                                out Vector2 contactPoint,
                                out Vector2 contactNormal,
                                out float nearestContactTime)
        {
            contactPoint = Vector2.Zero;
            contactNormal = Vector2.Zero;
            nearestContactTime = 0f;

            // Cache division
            var inverseRayDirection = Vector2.One / rayDirection;

            var targetPos = target.Pos();
            var targetSize = target.Size();

            // Calculate intersection with rectangle bounding axes
            var nearestContactPoint = (targetPos - rayOrigin) * inverseRayDirection;
            var furthestContactPoint = (targetPos + targetSize - rayOrigin) * inverseRayDirection;

            if (float.IsNaN(furthestContactPoint.X) || float.IsNaN(furthestContactPoint.Y)) return false;
            if (float.IsNaN(nearestContactPoint.X) || float.IsNaN(nearestContactPoint.Y)) return false;

            // Swap
            if (nearestContactPoint.X > furthestContactPoint.X) ObjectExtensions.Swap(ref nearestContactPoint.X, ref furthestContactPoint.X);

            // Swap
            if (nearestContactPoint.Y > furthestContactPoint.Y) ObjectExtensions.Swap(ref nearestContactPoint.Y, ref furthestContactPoint.Y);

            if (nearestContactPoint.X > furthestContactPoint.Y || nearestContactPoint.Y > furthestContactPoint.X) return false;

            // Nearest 'time' will be the first contact
            nearestContactTime = Math.Max(nearestContactPoint.X, nearestContactPoint.Y);

            // Furthest 'time' will contact on the opposite side of the target
            var furhestContactTime = Math.Min(furthestContactPoint.X, nearestContactPoint.Y);

            // If negative it is pointing away from target
            if (furhestContactTime < 0) return false;

            // Contact point of collision from parametric line equation
            contactPoint = rayOrigin + nearestContactTime * rayDirection;

            if (nearestContactPoint.X > nearestContactPoint.Y)
            {
                if (inverseRayDirection.X < 0f)
                {
                    contactNormal = new Vector2(1, 0);
                }
                else
                {
                    contactNormal = new Vector2(-1, 0);
                }
            }
            else if (nearestContactPoint.X < nearestContactPoint.Y)
            {
                if (inverseRayDirection.Y < 0f)
                {
                    contactNormal = new Vector2(0, 1);
                }
                else
                {
                    contactNormal = new Vector2(0, -1);
                }
            }

            return true;
        }

        private bool IsRectInRect(Rect source,
                                  Vector2 sourceVelocity,
                                  Rect target,
                                  out Vector2 contactPoint,
                                  out Vector2 contactNormal,
                                  out float contactTime,
                                  float elapsedTime)
        {
            contactPoint = Vector2.Zero;
            contactNormal = Vector2.Zero;
            contactTime = 0f;

            if (sourceVelocity == Vector2.Zero) return false;

            var expandedTarger = target.Add(source.Size());

            if (IsRayInRect(
                            source.Pos() + source.Size() / 2f,
                            sourceVelocity * elapsedTime,
                            expandedTarger,
                            out contactPoint,
                            out contactNormal,
                            out contactTime))
            {
                if (contactTime >= 0f && contactTime <= 1f) return true;
            }

            return false;
        }

        public bool TryGetOverlappingTiles(Rect rect, out IReadOnlyList<Rect> rects)
        {
            var tileRects = new List<Rect>();

            foreach (var tile in _tiles.Where(tile => tile != null && tile.TransformComponent != null))
            {
                var tileRect = tile.TransformComponent.Position.ToRect(32f, 32f);
                if (IsRectInRect(rect, tileRect))
                {
                    tileRects.Add(tileRect);
                }
            }

            rects = tileRects;

            return tileRects.Any();
        }

        private bool TryGetTileGameObject(Vector2 position, out GameObject gameObject, float elapsedTime)
        {
            var heroineRect = position.ToRect(_tileWidth, _tileHeight);
            gameObject = null;
            foreach (var tile in _tiles.Where(tile => tile != null && tile.TransformComponent != null))
            {
                var tileRect = new Rect(tile.TransformComponent.Position.X, tile.TransformComponent.Position.Y, _tileWidth, _tileHeight);
                if (IsRectInRect(heroineRect,
                                 _heroine.TransformComponent.Velocity,
                                 tileRect,
                                 out Vector2 contactPoint,
                                 out Vector2 contactNormal,
                                 out float contactTime,
                                 elapsedTime))
                {
                    gameObject = tile;
                    return true;
                }
            }

            return false;
        }

        private PlayerGameObject _heroine;

        private void InitializeHeroine(Windows.UI.Xaml.Window current)
        {
            var gameObject = new PlayerGameObject();
            new GraphicsComponent(gameObject, _canvasBitmaps[0], 0, 0);
            gameObject.GraphicsComponent = new GraphicsComponent(gameObject, _canvasBitmaps[0], 0, 96);
            gameObject.PhysicsComponent = new PhysicsComponent(gameObject);
            gameObject.InputComponent = new InputComponent(gameObject, current);
            gameObject.ColliderComponent = new ColliderComponent
            {
                Size = new Vector2(32f, 32f)
            };
            _graphicsComponents.Add(gameObject.GraphicsComponent);
            _heroine = gameObject;
        }

        private void InitializeTileMap()
        {
            TileMapIterator((data) =>
            {
                if (_mapData[data.offset] == 0) return;

                var transformComponent = new TransformComponent
                {
                    Position = new Vector2(data.x * _tileWidth, data.y * _tileHeight)
                };
                var gameObject = new GameObject(null, null, null, null, transformComponent);
                var graphicsComponent = CreateGraphicsComponent(gameObject, (TileType)_mapData[data.offset], _canvasBitmaps[0]);
                gameObject.GraphicsComponent = graphicsComponent;
                _graphicsComponents.Add(gameObject.GraphicsComponent);

                _tiles[data.offset] = gameObject;
            });
        }

        private void TileMapIterator(Action<(int offset, int x, int y)> action)
        {
            for (var x = 0; x < _mapTileWidth; x++)
            {
                for (var y = 0; y < _mapTileHeight; y++)
                {
                    var offset = y * _mapTileWidth + x;
                    action?.Invoke((offset, x, y));
                }
            }
        }

        public void Update(TimeSpan timeSpan)
        {
            TileMapIterator((data) =>
            {
                _tiles[data.offset]?.Update(this, timeSpan);
            });
            _heroine?.Update(this, timeSpan);
            // snap to ground if we are falling
            //if (TryGetGroundedTile(_heroine, out var tileGameObject, (float)timeSpan.TotalSeconds)
            //    && _heroine.TransformComponent.Velocity.Y > 0f)
            //{
            //    var xMemory = _heroine.TransformComponent.Position * Vector2.UnitX;
            //    _heroine.TransformComponent.Position = xMemory + (Vector2.UnitY * tileGameObject.TransformComponent.Position) - (32f * Vector2.UnitY);
            //    _heroine.TransformComponent.Velocity *= Vector2.UnitX;
            //    //newVelocity *= Vector2.UnitX;
            //}
        }

        public void Draw(CanvasDrawingSession canvasDrawingSession, TimeSpan timeSpan)
        {
            _graphicsComponents.ForEach(graphicsComponent => graphicsComponent.Update(canvasDrawingSession, timeSpan));
        }

        private GraphicsComponent CreateGraphicsComponent(GameObject gameObject, TileType tileType, CanvasBitmap canvasBitmap)
        {
            switch (tileType)
            {
                case TileType.Nothing:
                    return null;

                case TileType.PlatformLeft:
                    return new GraphicsComponent(gameObject, canvasBitmap, 0, 0);

                case TileType.PlatformCenter:
                    return new GraphicsComponent(gameObject, canvasBitmap, 32, 0);

                case TileType.PlatformRight:
                    return new GraphicsComponent(gameObject, canvasBitmap, 64, 0);

                case TileType.Floor:
                    return new GraphicsComponent(gameObject, canvasBitmap, 96, 0);

                default:
                    return null;
            }
        }
    }

    public enum TileType
    {
        Nothing,
        PlatformLeft,
        PlatformCenter,
        PlatformRight,
        Floor,
    }
}