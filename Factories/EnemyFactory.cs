using Microsoft.Graphics.Canvas;
using System;
using System.Numerics;
using uwpPlatformer.Components;
using uwpPlatformer.GameObjects;
using uwpPlatformer.Numerics;

namespace uwpPlatformer.Factories
{
    public class EnemyFactory
    {
        private readonly IGameObjectManager _gameObjectManager;

        public EnemyFactory(IGameObjectManager gameObjectManager)
        {
            _gameObjectManager = gameObjectManager;
        }

        public GameObject CreateFlyingEnemy(CanvasBitmap canvasBitmap, Vector2 position, Vector2 size, BoundingBox bounds)
        {
            var gameObject = _gameObjectManager.CreateGameObject();
            gameObject.GetComponent<TransformComponent>().Position = position;

            //gameObject.AddComponent(new ShapeGraphicsComponent(gameObject, ShapeType.Rectangle, Colors.Pink, size));
            gameObject.AddOrUpdateComponent(new AnimatedGraphicsComponent(gameObject, canvasBitmap, new[] { new BoundingBox(new Vector2(0, 0), size) }, TimeSpan.Zero));
            gameObject.AddOrUpdateComponent(new PerlinMovementComponent(gameObject, bounds, 0f, 10000f));
            gameObject.AddOrUpdateComponent(new PhysicsComponent(gameObject) { Gravity = Vector2.Zero, Drag = 0f });
            gameObject.AddOrUpdateComponent(new ColliderComponent(gameObject) { Size = size, CollisionType = ColliderComponent.CollisionTypes.DynamicWorld });
            gameObject.AddOrUpdateComponent(new EnemyComponent(gameObject));

            return gameObject;
        }
    }
}
