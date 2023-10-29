using Microsoft.Graphics.Canvas;
using System;
using System.Numerics;
using uwpPlatformer.Components;
using uwpPlatformer.GameObjects;
using uwpPlatformer.Models;

namespace uwpPlatformer.Factories
{
    public class TileFactory
    {
        private readonly IGameObjectManager _gameObjectManager;

        public TileFactory(IGameObjectManager gameObjectManager)
        {
            _gameObjectManager = gameObjectManager;
        }

        public GameObject CreateTile(TileSet tileSet, Vector2 position, int tileId)
        {
            var gameObject = _gameObjectManager.CreateGameObject();
            var transformComponent = new TransformComponent(gameObject)
            {
                Position = position
            };

            gameObject.AddOrUpdateComponent(transformComponent);

            var graphicsComponent = CreateGraphicsComponent(gameObject, tileSet, tileId);
            gameObject.AddOrUpdateComponent(graphicsComponent);
            // TODO: The currently loaded tiles are all collidables, so when making the sky or what ever we need to fix this
            gameObject.AddOrUpdateComponent(new ColliderComponent(gameObject)
            {
                Size = new Vector2(tileSet.TileAtlas.TileWidth, tileSet.TileAtlas.TileHeight),
                CollisionType = ColliderComponent.CollisionTypes.StaticPlatform
            });

            return gameObject;
        }

        private AnimatedGraphicsComponent CreateGraphicsComponent(GameObject gameObject, TileSet tileSet, int tileId)
        {
            return new AnimatedGraphicsComponent(gameObject, tileSet.TileAtlas.Bitmap, new[] { tileId - tileSet.FirstGid }, TimeSpan.Zero, tileSet.TileAtlas.Columns);
        }
    }
}
