using Microsoft.Graphics.Canvas;
using System;
using System.IO;
using uwpKarate.Components;
using uwpKarate.Models;

namespace uwpKarate.GameObjects
{
    public class World
    {
        private readonly int _mapTileWidth = 14;
        private readonly int _mapTileHeight = 10;
        private const int _tileWidth = 32;
        private const int _tileHeight = 32;
        private readonly CanvasBitmap _canvasBitmap;
        private readonly Map _map;
        private GameObject[] _tiles;
        private GraphicsComponent _wallTile;
        private GraphicsComponent _floorTile;
        private GraphicsComponent _platformTile;

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

        public World(CanvasBitmap canvasBitmap, Map map)
        {
            _canvasBitmap = canvasBitmap;
            _map = map;
            _mapData = _map.Layers[0].Data;

            _mapTileWidth = _map.Width;
            _mapTileHeight = _map.Height;
            _tiles = new GameObject[_mapTileWidth * _mapTileHeight];

            InitializeTileMap();
        }

        private void InitializeTileMap()
        {
            for (var x = 0; x < _mapTileWidth; x++)
            {
                for (var y = 0; y < _mapTileHeight; y++)
                {
                    try
                    {
                        var offset = y * _mapTileWidth + x;
                        if (_mapData[offset] == 0)
                        {
                            continue;
                        }

                        var transformComponent = new TransformComponent
                        {
                            XPos = x * _tileWidth,
                            YPos = y * _tileHeight
                        };
                        var gameObject = new GameObject(null, null, null, transformComponent);
                        var graphicsComponent = CreateGraphicsComponent(gameObject, (TileType)_mapData[offset], _canvasBitmap);
                        gameObject.GraphicsComponent = graphicsComponent;

                        _tiles[offset] = gameObject;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public void Update(TimeSpan timeSpan)
        {
            for (var y = 0; y < _mapTileHeight; y++)
            {
                for (var x = 0; x < _mapTileWidth; x++)
                {
                    var offset = y * _mapTileWidth + x;
                    _tiles[offset]?.Update(this, timeSpan);
                }
            }
        }

        public void Draw(CanvasDrawingSession canvasDrawingSession)
        {
            for (var y = 0; y < _mapTileHeight; y++)
            {
                for (var x = 0; x < _mapTileWidth; x++)
                {
                    var offset = y * _mapTileWidth + x;
                    _tiles[offset]?.Draw(canvasDrawingSession);
                }
            }
        }

        private GraphicsComponent CreateGraphicsComponent(GameObject gameObject, TileType tileType, CanvasBitmap canvasBitmap)
        {
            switch (tileType)
            {
                case TileType.Nothing:
                    return null;

                case TileType.PlatformLeft:
                case TileType.PlatformCenter:
                case TileType.PlatformRight:
                    return new GraphicsComponent(gameObject, canvasBitmap, 32, 0);

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