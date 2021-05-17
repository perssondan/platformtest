using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Numerics;
using uwpKarate.Components;
using uwpKarate.Factories;
using uwpKarate.Models;
using uwpKarate.Systems;
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

        private ColliderSystem _colliderSystem = ColliderSystem.Instance;
        private MoveSystem _moveSystem = MoveSystem.Instance;
        private PhysicsSystem _physicsSystem = PhysicsSystem.Instance;
        private InputSystem _inputSystem = InputSystem.Instance;
        private GraphicsSystem _graphicsSystem = GraphicsSystem.Instance;
        private ParticleSystem _particleSystem = ParticleSystem.Instance;
        private PlayerSystem _playerSystem = PlayerSystem.Instance;

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

        public World(CanvasBitmap[] canvasBitmaps, Map map, TileAtlas[] tileAtlases, Windows.UI.Xaml.Window current)
        {
            _inputSystem.Current = current;
            _canvasBitmaps = canvasBitmaps;
            _map = map;
            _tileAtlases = tileAtlases;
            _mapData = _map.Layers[0].Data;

            _mapTileWidth = _map.Width;
            _mapTileHeight = _map.Height;
            _tiles = new GameObject[_mapTileWidth * _mapTileHeight];

            InitializeWorldBoundaries();
            InitializeTileMap();
            InitializeHeroine();
        }

        public int WorldPixelHeight => _map.Height * _map.TileHeight;
        public int WorldPixelWidth => _map.Width * _map.TileWidth;

        public Rect WorldRect => new Rect(0, 0, WorldPixelWidth, WorldPixelHeight);

        private void InitializeHeroine()
        {
            _heroFactory.CreateHero(_canvasBitmaps[0], new Vector2(288f, 256f), new Vector2(_tileWidth - 1, _tileHeight - 1));
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
            TileMapIterator((data) =>
            {
                if (_mapData[data.offset] == 0) return;

                _tiles[data.offset] = _tileFactory.CreateTile(_canvasBitmaps[0],
                                                              new Vector2(data.x * _tileWidth, data.y * _tileHeight),
                                                              new Vector2(_tileWidth, _tileHeight),
                                                              (TileType)_mapData[data.offset]);
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

        public void Update(TimeSpan deltaTime)
        {
            _inputSystem.Update(deltaTime);
            _graphicsSystem.Update(deltaTime);
            _playerSystem.Update(deltaTime);
            _physicsSystem.Update(deltaTime);
            _particleSystem.Update(deltaTime);
            _colliderSystem.Update(deltaTime);

            // If we still have collisions, resolve them now!
            _colliderSystem.ResolveCollisions(deltaTime);
            // All done, move game objects
            _moveSystem.Update(deltaTime);
        }

        public void Draw(CanvasDrawingSession canvasDrawingSession, TimeSpan timeSpan)
        {
            _graphicsSystem.Draw(canvasDrawingSession, timeSpan);
        }
    }
}
