using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Numerics;
using uwpPlatformer.Components;
using uwpPlatformer.Factories;
using uwpPlatformer.Models;
using Windows.Foundation;

namespace uwpPlatformer.GameObjects
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
        private EnemyFactory _enemyFactory = new EnemyFactory();

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
            CreateFlyingEnemy();
        }

        public int WorldPixelHeight => _map.Height * _map.TileHeight;
        public int WorldPixelWidth => _map.Width * _map.TileWidth;

        private void CreateFlyingEnemy()
        {
            _enemyFactory.CreateFlyingEnemy(_canvasBitmaps[1], new Vector2(100f, 100f), new Vector2(_tileAtlases[1].TileWidth - 1, _tileAtlases[1].TileHeight - 1), new Rect(0, 0, WorldPixelWidth, WorldPixelHeight));
        }

        private void InitializeHeroine()
        {
            _heroFactory.CreateHero(_canvasBitmaps[0], new Vector2(288f, 256f), new Vector2(_tileAtlases[0].TileWidth - 1, _tileAtlases[0].TileHeight - 1));
        }

        private void InitializeWorldBoundaries()
        {
            var leftBoundary = new GameObject();

            leftBoundary.AddOrUpdateComponent(new TransformComponent(leftBoundary)
            {
                Position = new Vector2(-32f, -32f)
            });
            leftBoundary.AddOrUpdateComponent(new ColliderComponent(leftBoundary)
            {
                CollisionType = ColliderComponent.CollisionTypes.StaticWorld,
                Size = new Vector2(32f, WorldPixelHeight + 32f + 32f)
            });

            var rightBoundary = new GameObject();

            rightBoundary.AddOrUpdateComponent(new TransformComponent(rightBoundary)
            {
                Position = new Vector2(WorldPixelWidth, -32f)
            });
            rightBoundary.AddOrUpdateComponent(new ColliderComponent(rightBoundary)
            {
                CollisionType = ColliderComponent.CollisionTypes.StaticWorld,
                Size = new Vector2(32f, WorldPixelHeight + 32f + 32f)
            });

            var topBoundary = new GameObject();

            topBoundary.AddOrUpdateComponent(new TransformComponent(topBoundary)
            {
                Position = new Vector2(-32f, -32f)
            });
            topBoundary.AddOrUpdateComponent(new ColliderComponent(topBoundary)
            {
                CollisionType = ColliderComponent.CollisionTypes.StaticWorld,
                Size = new Vector2(WorldPixelWidth + 32f + 32f, 32f)
            });

            var bottomBoundary = new GameObject();

            bottomBoundary.AddOrUpdateComponent(new TransformComponent(bottomBoundary)
            {
                Position = new Vector2(-32f, WorldPixelHeight)
            });
            bottomBoundary.AddOrUpdateComponent(new ColliderComponent(bottomBoundary)
            {
                CollisionType = ColliderComponent.CollisionTypes.StaticWorld,
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
