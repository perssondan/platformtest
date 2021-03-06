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

        public GameObject CreateTile(CanvasBitmap canvasBitmap, Vector2 position, Vector2 size, TileType tileType)
        {
            var gameObject = _gameObjectManager.CreateGameObject();
            var transformComponent = new TransformComponent(gameObject)
            {
                Position = position
            };

            gameObject.AddOrUpdateComponent(transformComponent);

            var graphicsComponent = CreateGraphicsComponent(gameObject, tileType, canvasBitmap);
            gameObject.AddOrUpdateComponent(graphicsComponent);
            // TODO: The currently loaded tiles are all collidables, so when making the sky or what ever we need to fix this
            gameObject.AddOrUpdateComponent(new ColliderComponent(gameObject)
            {
                Size = size,
                CollisionType = ColliderComponent.CollisionTypes.StaticPlatform
            });

            return gameObject;
        }

        private AnimatedGraphicsComponent CreateGraphicsComponent(GameObject gameObject, TileType tileType, CanvasBitmap canvasBitmap)
        {
            switch (tileType)
            {
                case TileType.Nothing:
                    return null;

                case TileType.PlatformLeft:
                    return new AnimatedGraphicsComponent(gameObject, canvasBitmap, new[] { 0 }, TimeSpan.Zero);

                case TileType.PlatformCenter:
                    return new AnimatedGraphicsComponent(gameObject, canvasBitmap, new[] { 1 }, TimeSpan.Zero);

                case TileType.PlatformRight:
                    return new AnimatedGraphicsComponent(gameObject, canvasBitmap, new[] { 2 }, TimeSpan.Zero);

                case TileType.Floor:
                    return new AnimatedGraphicsComponent(gameObject, canvasBitmap, new[] { 3 }, TimeSpan.Zero);

                default:
                    return null;
            }
        }
    }
}
