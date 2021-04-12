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
        }

        public int WorldPixelHeight => _map.Height * _map.TileHeight;
        public int WorldPixelWidth => _map.Width * _map.TileWidth;

        public Rect WorldRect => new Rect(0, 0, WorldPixelWidth, WorldPixelHeight);

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