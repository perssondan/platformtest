using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Numerics;
using uwpKarate.Components;
using uwpKarate.Factories;
using uwpKarate.Models;
using Windows.Foundation;

namespace uwpKarate.GameObjects
{
    public class World
    {
        private readonly int _horizontalTileCount = 14;
        private readonly int _verticalTileCount = 10;
        private readonly IReadOnlyList<CanvasBitmap> _canvasBitmaps;
        private readonly Map _map;
        private readonly TileAtlas[] _tileAtlases;
        private GameObject[] _tiles;

        private HeroFactory _heroFactory = new HeroFactory();
        private TileFactory _tileFactory = new TileFactory();

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

        public World(CanvasBitmap[] canvasBitmaps, Map map, TileAtlas[] tileAtlases)
        {
            _canvasBitmaps = canvasBitmaps;
            _map = map;
            _tileAtlases = tileAtlases;
            _mapData = _map.Layers[0].Data;

            _horizontalTileCount = _map.Width;
            _verticalTileCount = _map.Height;
            _tiles = new GameObject[_horizontalTileCount * _verticalTileCount];

            InitializeWorldBoundaries();
            InitializeTileMap();
            InitializeHeroine();
        }

        public int WorldPixelHeight => _map.Height * _map.TileHeight;
        public int WorldPixelWidth => _map.Width * _map.TileWidth;

        public Rect WorldRect => new Rect(0, 0, WorldPixelWidth, WorldPixelHeight);

        private void InitializeHeroine()
        {
            _heroFactory.CreateHero(_canvasBitmaps[0], new Vector2(288f, 256f), new Vector2(_tileAtlases[0].TileWidth - 1, _tileAtlases[0].TileHeight - 1));
        }

        private void InitializeWorldBoundaries()
        {
            var leftBoundary = new GameObject();

            leftBoundary.AddComponent(new TransformComponent(leftBoundary)
            {
                Position = new Vector2(-32f, -32f)
            });
            leftBoundary.AddComponent(new ColliderComponent(leftBoundary)
            {
                CollisionType = ColliderComponent.CollisionTypes.Static,
                Size = new Vector2(32f, WorldPixelHeight + 32f + 32f)
            });

            var rightBoundary = new GameObject();

            rightBoundary.AddComponent(new TransformComponent(rightBoundary)
            {
                Position = new Vector2(WorldPixelWidth, -32f)
            });
            rightBoundary.AddComponent(new ColliderComponent(rightBoundary)
            {
                CollisionType = ColliderComponent.CollisionTypes.Static,
                Size = new Vector2(32f, WorldPixelHeight + 32f + 32f)
            });

            var topBoundary = new GameObject();

            topBoundary.AddComponent(new TransformComponent(topBoundary)
            {
                Position = new Vector2(-32f, -32f)
            });
            topBoundary.AddComponent(new ColliderComponent(topBoundary)
            {
                CollisionType = ColliderComponent.CollisionTypes.Static,
                Size = new Vector2(WorldPixelWidth + 32f + 32f, 32f)
            });

            var bottomBoundary = new GameObject();

            bottomBoundary.AddComponent(new TransformComponent(bottomBoundary)
            {
                Position = new Vector2(-32f, WorldPixelHeight)
            });
            bottomBoundary.AddComponent(new ColliderComponent(bottomBoundary)
            {
                CollisionType = ColliderComponent.CollisionTypes.Static,
                Size = new Vector2(WorldPixelWidth + 32f + 32f, 32f)
            });
        }

        private void InitializeTileMap()
        {
            var tileWidth = _tileAtlases[0].TileWidth;
            var tileHeight = _tileAtlases[0].TileHeight;
            TileMapIterator((data) =>
            {
                if (_mapData[data.offset] == 0) return;

                _tiles[data.offset] = _tileFactory.CreateTile(_canvasBitmaps[0],
                                                              new Vector2(data.x * tileWidth, data.y * tileHeight),
                                                              new Vector2(tileWidth, tileHeight),
                                                              (TileType)_mapData[data.offset]);
            });
        }

        private void TileMapIterator(Action<(int offset, int x, int y)> action)
        {
            for (var x = 0; x < _horizontalTileCount; x++)
            {
                for (var y = 0; y < _verticalTileCount; y++)
                {
                    var offset = y * _horizontalTileCount + x;
                    action?.Invoke((offset, x, y));
                }
            }
        }
    }
}
