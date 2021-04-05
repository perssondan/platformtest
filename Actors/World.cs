using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uwpKarate.Models;

namespace uwpKarate.Actors
{
    public class World
    {
        private const int _mapWidth = 14;
        private const int _mapHeight = 10;
        private const int _tileWidth = 32;
        private const int _tileHeight = 32;
        private GameObject[,] _tiles = new GameObject[_mapWidth, _mapHeight];
        private GraphicsComponent _wallTile;
        private GraphicsComponent _floorTile;
        private GraphicsComponent _platformTile;


        private int[,] _map = new[,]
        {
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 3},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 3},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 3},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 3},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 3},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 3},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 3},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 3},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 3},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 3},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 3},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 3},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 3},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 3}
        };

        public World(CanvasBitmap canvasBitmap)
        {
            _floorTile = new GraphicsComponent(canvasBitmap, 96, 0);
            _platformTile = new GraphicsComponent(canvasBitmap, 32, 0);
            for (var y = 0; y < _mapHeight; y++)
            {
                for (var x = 0; x < _mapWidth; x++)
                {
                    try
                    {
                        _tiles[x, y] = new GameObject(x * _tileWidth, y * _tileHeight, GetGraphicsComponent((TileType)_map[x, y]));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public void Draw(CanvasDrawingSession canvasDrawingSession)
        {
            for (var y = 0; y < _mapHeight; y++)
            {
                for (var x = 0; x < _mapWidth; x++)
                {
                    _tiles[x, y]?.GraphicsComponent?.Draw(canvasDrawingSession, _tiles[x, y]);
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
