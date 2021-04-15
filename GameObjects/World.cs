using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using uwpKarate.Components;
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

        private List<GraphicsComponent> _graphicsComponents = new List<GraphicsComponent>();

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

        public bool TryGetGroundedTile(GameObject gameObject, out GameObject tileGameObject)
        {
            if (TryGetTileGameObject(gameObject.TransformComponent.Position, out tileGameObject))
            {
                if (tileGameObject != null)
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryGetTileGameObject(Vector2 position, out GameObject gameObject)
        {
            // TODO: Get collision points from heroin
            var topLeft = position.ToPoint();
            var topRight = (position + new Vector2(_tileWidth, 0)).ToPoint();
            var bottomLeft = (position + new Vector2(0, _tileHeight)).ToPoint();
            var bottomRight = (position + new Vector2(_tileWidth, _tileHeight)).ToPoint();
            gameObject = null;
            foreach (var tile in _tiles.Where(tile => tile != null && tile.TransformComponent != null))
            {
                var rect = new Rect(tile.TransformComponent.Position.X, tile.TransformComponent.Position.Y, _tileWidth, _tileHeight);
                if (rect.Contains(topLeft) || rect.Contains(topRight) || rect.Contains(bottomLeft) || rect.Contains(bottomRight))
                {
                    gameObject = tile;
                    return true;
                }
            }

            return false;
        }

        private GameObject _heroine;

        private void InitializeHeroine(Windows.UI.Xaml.Window current)
        {
            var gameObject = new GameObject();
            new GraphicsComponent(gameObject, _canvasBitmaps[0], 0, 0);
            gameObject.GraphicsComponent = new GraphicsComponent(gameObject, _canvasBitmaps[0], 0, 96);
            gameObject.PhysicsComponent = new PhysicsComponent(gameObject);
            gameObject.InputComponent = new InputComponent(gameObject, current);
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
                var gameObject = new GameObject(null, null, null, transformComponent);
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
        }

        public void Draw(CanvasDrawingSession canvasDrawingSession)
        {
            _graphicsComponents.ForEach(graphicsComponent => graphicsComponent.Draw(canvasDrawingSession));
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