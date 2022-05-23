using Microsoft.Graphics.Canvas;
using System;
using System.Numerics;
using uwpPlatformer.Components;
using uwpPlatformer.GameObjects;
using uwpPlatformer.Numerics;
using Windows.Foundation;

namespace uwpPlatformer.Factories
{
    public class HeroFactory
    {
        private readonly IGameObjectManager _gameObjectManager;

        public HeroFactory(IGameObjectManager gameObjectManager)
        {
            _gameObjectManager = gameObjectManager;
        }

        public GameObject CreateHero(CanvasBitmap canvasBitmap, Vector2 position, Vector2 size)
        {
            var gameObject = _gameObjectManager.CreateGameObject();
            gameObject.AddOrUpdateComponent(new TransformComponent(gameObject)
            {
                Position = position
            });
            var sourceRects = new BoundingBox[]
            {
                new BoundingBox(0,32f,32,32),
                new BoundingBox(32f,32f,32f,32f),
                new BoundingBox(64f,32f,32f,32f),
                new BoundingBox(96f,32f,32f,32f),
            };
            var animatedGraphicsComponent = new AnimatedGraphicsComponent(gameObject, canvasBitmap, sourceRects, TimeSpan.FromMilliseconds(150));
            gameObject.AddOrUpdateComponent(animatedGraphicsComponent);
            gameObject.AddOrUpdateComponent(new PhysicsComponent(gameObject) { Position = position });
            gameObject.AddOrUpdateComponent(new InputComponent(gameObject));
            gameObject.AddOrUpdateComponent(new PlayerComponent(gameObject));
            gameObject.AddOrUpdateComponent(new HeroComponent(gameObject));
            var collider = new ColliderComponent(gameObject)
            {
                Size = size,
                CollisionType = ColliderComponent.CollisionTypes.Dynamic
            };
            gameObject.AddOrUpdateComponent(collider);

            return gameObject;
        }
    }
}
