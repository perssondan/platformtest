using Microsoft.Graphics.Canvas;
using System;
using uwpKarate.Models;

namespace uwpKarate.Actors
{
    public class World
    {
        private const int _mapTileWidth = 14;
        private const int _mapTileHeight = 10;
        private const int _tileWidth = 32;
        private const int _tileHeight = 32;
        private GameObject[] _tiles = new GameObject[_mapTileWidth * _mapTileHeight];
        private GraphicsComponent _wallTile;
        private GraphicsComponent _floorTile;
        private GraphicsComponent _platformTile;

        private int[] _map = new[]
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

        public World(CanvasBitmap canvasBitmap)
        {
            _floorTile = new GraphicsComponent(canvasBitmap, 96, 0);
            _platformTile = new GraphicsComponent(canvasBitmap, 32, 0);
            for (var x = 0; x < _mapTileWidth; x++)
            {
                for (var y = 0; y < _mapTileHeight; y++)
                {
                    try
                    {
                        var offset = (y * _mapTileWidth) + x;
                        _tiles[offset] = new GameObject(x * _tileWidth, y * _tileHeight, GetGraphicsComponent((TileType)_map[offset]));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public void Draw(CanvasDrawingSession canvasDrawingSession)
        {
            for (var y = 0; y < _mapTileHeight; y++)
            {
                for (var x = 0; x < _mapTileWidth; x++)
                {
                    var offset = (y * _mapTileWidth) + x;
                    _tiles[offset]?.GraphicsComponent?.Draw(canvasDrawingSession, _tiles[offset]);
                }
            }
        }

        private GraphicsComponent GetGraphicsComponent(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Nothing:
                    return null;

                case TileType.Wall:
                    return _wallTile;

                case TileType.Platform:
                    return _platformTile;

                case TileType.Floor:
                    return _floorTile;

                default:
                    return null;
            }
        }
    }

    public enum TileType
    {
        Nothing,
        Wall,
        Platform,
        Floor
    }
}