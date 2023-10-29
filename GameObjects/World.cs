using System;
using System.Linq;
using System.Numerics;
using uwpPlatformer.Components;
using uwpPlatformer.Factories;
using uwpPlatformer.Platform;

namespace uwpPlatformer.GameObjects
{
    /// <summary>
    /// Handle setting up the platform scene
    /// </summary>
    public class World
    {
        private readonly IGameAssetsProvider _gameAssetsProvider;
        private readonly IGameObjectManager _gameObjectManager;

        private TileFactory _tileFactory;

        public World(
            IGameAssetsProvider gameAssetsProvider,
            IGameObjectManager gameObjectManager)
        {
            _gameAssetsProvider = gameAssetsProvider;
            _gameObjectManager = gameObjectManager;

            _tileFactory = new TileFactory(_gameObjectManager);

            InitializeWorldBoundaries();
            InitializeTileMap();
        }

        /// <summary>
        /// Set up boundaries such that characters can't walk outside the screen
        /// </summary>
        private void InitializeWorldBoundaries()
        {
            var topLeft = new Vector2(-32f, -32f);
            var topRight = new Vector2(_gameAssetsProvider.Map.MapWidth, -32f);
            var bottomLeft = new Vector2(-32f, _gameAssetsProvider.Map.MapHeight);

            var leftBoundary = _gameObjectManager.CreateGameObject();
            leftBoundary.AddOrUpdateComponent(new TransformComponent(leftBoundary)
            {
                Position = topLeft
            });
            leftBoundary.AddOrUpdateComponent(new ColliderComponent(leftBoundary)
            {
                CollisionType = ColliderComponent.CollisionTypes.StaticWorld,
                Size = new Vector2(32f, _gameAssetsProvider.Map.MapHeight + 32f + 32f)
            });

            var rightBoundary = _gameObjectManager.CreateGameObject();
            rightBoundary.AddOrUpdateComponent(new TransformComponent(rightBoundary)
            {
                Position = topRight
            });
            rightBoundary.AddOrUpdateComponent(new ColliderComponent(rightBoundary)
            {
                CollisionType = ColliderComponent.CollisionTypes.StaticWorld,
                Size = new Vector2(32f, _gameAssetsProvider.Map.MapHeight + 32f + 32f)
            });

            var topBoundary = _gameObjectManager.CreateGameObject();
            topBoundary.AddOrUpdateComponent(new TransformComponent(topBoundary)
            {
                Position = topLeft
            });
            topBoundary.AddOrUpdateComponent(new ColliderComponent(topBoundary)
            {
                CollisionType = ColliderComponent.CollisionTypes.StaticWorld,
                Size = new Vector2(_gameAssetsProvider.Map.MapWidth + 32f + 32f, 32f)
            });

            var bottomBoundary = _gameObjectManager.CreateGameObject();
            bottomBoundary.AddOrUpdateComponent(new TransformComponent(bottomBoundary)
            {
                Position = bottomLeft
            });
            bottomBoundary.AddOrUpdateComponent(new ColliderComponent(bottomBoundary)
            {
                CollisionType = ColliderComponent.CollisionTypes.StaticWorld,
                Size = new Vector2(_gameAssetsProvider.Map.MapWidth + 32f + 32f, 32f)
            });
        }

        private void InitializeTileMap()
        {
            var layersData = _gameAssetsProvider.Map.Layers
                .Where(x => x.LayerType.Equals("tileLayer", StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Data ?? Array.Empty<int>())
                .ToArray();

            foreach (var mapData in layersData)
            {
                for (var index = 0; index < mapData.Length; index++)
                {
                    var horizontalTileIndex = index % _gameAssetsProvider.Map.Width;
                    var verticalTileIndex = index / _gameAssetsProvider.Map.Width;

                    var position = new Vector2(horizontalTileIndex * _gameAssetsProvider.Map.TileWidth, verticalTileIndex * _gameAssetsProvider.Map.TileHeight);
                    AddTileGameObject(mapData[index], position);
                }
            }
        }

        private void AddTileGameObject(int tileId, Vector2 position)
        {
            if (tileId == 0)
                return;

            if (!_gameAssetsProvider.TryGetTileSet(tileId, out var tileSet))
                return;

            _tileFactory.CreateTile(tileSet, position, tileId);
        }
    }
}
