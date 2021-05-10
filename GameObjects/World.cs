using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using uwpKarate.Components;
using uwpKarate.Extensions;
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

        private void InitializeHeroine(Windows.UI.Xaml.Window current)
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
            gameObject.AddComponent<GraphicsComponentBase>(animatedGraphicsComponent);
            gameObject.AddComponent(new PhysicsComponent(gameObject));
            gameObject.AddComponent(new InputComponent(gameObject, current));
            gameObject.AddComponent(new PlayerComponent(gameObject));
            gameObject.AddComponent(new ColliderComponent(gameObject)
            {
                Size = new Vector2(_tileWidth, _tileHeight),
                CollisionType = ColliderComponent.CollisionTypes.Dynamic
            });
            _heroine = gameObject;
            _heroine.TransformComponent.Position = new Vector2(288f, 256f);
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
                gameObject.AddComponent<GraphicsComponentBase>(graphicsComponent);
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

        public void Update(TimeSpan timeSpan)
        {
            _heroine?.Update(this, timeSpan);
            _physicsSystem.Update(this, timeSpan);
            _colliderSystem.Update(this, timeSpan);
            // We might have collisions, resolve them now!
            // enforce player is inside world
            _physicsSystem.Resolve(this, timeSpan);
            _moveSystem.Update(this, timeSpan);
        }

        public void Draw(CanvasDrawingSession canvasDrawingSession, TimeSpan timeSpan)
        {
            GraphicsComponentManager.Instance.Components.ForEach(graphicsComponent => graphicsComponent.Update(canvasDrawingSession, timeSpan));
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