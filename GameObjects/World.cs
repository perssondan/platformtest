using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Numerics;
using uwpKarate.Components;
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
        private PlayerGameObject _heroine;

        private ColliderSystem _colliderSystem = ColliderSystem.Instance;
        private MoveSystem _moveSystem = MoveSystem.Instance;
        private PhysicsSystem _physicsSystem = PhysicsSystem.Instance;
        private InputSystem _inputSystem = InputSystem.Instance;
        private GraphicsSystem _graphicsSystem = GraphicsSystem.Instance;
        private ParticleSystem _particleSystem = ParticleSystem.Instance;

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
            var gameObject = new PlayerGameObject();
            var sourceRects = new Rect[]
            {
                new Rect(0,32f,32,32),
                new Rect(32f,32f,32f,32f),
                new Rect(64f,32f,32f,32f),
                new Rect(96f,32f,32f,32f),
            };
            var animatedGraphicsComponent = new AnimatedGraphicsComponent(gameObject, _canvasBitmaps[0], sourceRects, TimeSpan.FromMilliseconds(150));
            //gameObject.AddComponent(new GraphicsComponent(gameObject, _canvasBitmaps[0], 0, 96));
            gameObject.AddComponent(animatedGraphicsComponent);
            gameObject.AddComponent(new PhysicsComponent(gameObject));
            gameObject.AddComponent(new InputComponent(gameObject));
            gameObject.AddComponent(new PlayerComponent(gameObject));
            gameObject.AddComponent(new ColliderComponent(gameObject)
            {
                Size = new Vector2(_tileWidth-1, _tileHeight-1),
                CollisionType = ColliderComponent.CollisionTypes.Dynamic
            });
            _heroine = gameObject;
            _heroine.TransformComponent.Position = new Vector2(288f, 256f);
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

                var gameObject = new GameObject();
                var transformComponent = new TransformComponent(gameObject)
                {
                    Position = new Vector2(data.x * _tileWidth, data.y * _tileHeight)
                };

                gameObject.AddComponent(transformComponent);

                var graphicsComponent = CreateGraphicsComponent(gameObject, (TileType)_mapData[data.offset], _canvasBitmaps[0]);
                gameObject.AddComponent(graphicsComponent);
                // TODO: The currently loaded tiles are all collidables, so when making the sky or what ever we need to fix this
                gameObject.AddComponent(new ColliderComponent(gameObject)
                {
                    Size = new Vector2(_tileWidth, _tileHeight),
                    CollisionType = ColliderComponent.CollisionTypes.Static
                });

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

        public void Update(TimeSpan deltaTime)
        {
            _inputSystem.Update(this, deltaTime);
            _graphicsSystem.Update(this, deltaTime);
            _heroine?.Update(this, deltaTime);
            _physicsSystem.Update(this, deltaTime);
            _particleSystem.Update(this, deltaTime);
            _colliderSystem.Update(this, deltaTime);

            // If we still have collisions, resolve them now!
            _colliderSystem.ResolveCollisions(deltaTime);
            //_physicsSystem.Resolve(this, deltaTime);
            _moveSystem.Update(this, deltaTime);
        }

        public void Draw(CanvasDrawingSession canvasDrawingSession, TimeSpan timeSpan)
        {
            _graphicsSystem.Draw(canvasDrawingSession, timeSpan);
        }

        private AnimatedGraphicsComponent CreateGraphicsComponent(GameObject gameObject, TileType tileType, CanvasBitmap canvasBitmap)
        {
            switch (tileType)
            {
                case TileType.Nothing:
                    return null;

                case TileType.PlatformLeft:
                    return new AnimatedGraphicsComponent(gameObject, canvasBitmap, new[] { new Rect(0, 0, 32f, 32f) }, TimeSpan.Zero);

                case TileType.PlatformCenter:
                    return new AnimatedGraphicsComponent(gameObject, canvasBitmap, new[] { new Rect(32f, 0, 32f, 32f) }, TimeSpan.Zero);

                case TileType.PlatformRight:
                    return new AnimatedGraphicsComponent(gameObject, canvasBitmap, new[] { new Rect(64f, 0, 32f, 32f) }, TimeSpan.Zero);

                case TileType.Floor:
                    return new AnimatedGraphicsComponent(gameObject, canvasBitmap, new[] { new Rect(96f, 0, 32f, 32f) }, TimeSpan.Zero);

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
