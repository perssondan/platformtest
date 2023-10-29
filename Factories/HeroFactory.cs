using Microsoft.Graphics.Canvas;
using System;
using System.Linq;
using System.Numerics;
using uwpPlatformer.Components;
using uwpPlatformer.GameObjects;
using uwpPlatformer.Models;

namespace uwpPlatformer.Factories
{
    public class HeroFactory
    {
        private readonly IGameObjectManager _gameObjectManager;

        public HeroFactory(IGameObjectManager gameObjectManager)
        {
            _gameObjectManager = gameObjectManager;
        }

        public GameObject CreateHero(TileSet tileSet, Vector2 position)
        {
            var gameObject = _gameObjectManager.CreateGameObject();
            gameObject.AddOrUpdateComponent(new TransformComponent(gameObject)
            {
                Position = position
            });
            var animatedGraphicsComponent = new AnimatedGraphicsComponent(gameObject, tileSet.TileAtlas.Bitmap, PlayerComponent.StationarySourceSpriteIndexes, TimeSpan.FromMilliseconds(150));
            gameObject.AddOrUpdateComponent(animatedGraphicsComponent);
            gameObject.AddOrUpdateComponent(new PhysicsComponent(gameObject) { Position = position });
            gameObject.AddOrUpdateComponent(new InputComponent(gameObject));
            gameObject.AddOrUpdateComponent(new PlayerComponent(gameObject));
            gameObject.AddOrUpdateComponent(new HeroComponent(gameObject));
            var collider = new ColliderComponent(gameObject)
            {
                Size = GetColliderSize(tileSet, 8),
                CollisionType = ColliderComponent.CollisionTypes.Dynamic
            };
            gameObject.AddOrUpdateComponent(collider);
            gameObject.AddOrUpdateComponent(new TagComponent(gameObject, "hero"));

            return gameObject;
        }

        private Vector2 GetColliderSize(TileSet tileSet, int tileId)
        {
            var tile = tileSet.TileAtlas.Tiles
                .FirstOrDefault(t => t.Id == tileId);

            var collisionObject = tile.ObjectGroup?.CollisionObjects?.FirstOrDefault();
            if (collisionObject is null)
            {
                return new Vector2(tileSet.TileAtlas.TileWidth, tileSet.TileAtlas.TileHeight);
            }

            return new Vector2(collisionObject.Width, collisionObject.Height);
        }
    }
}
