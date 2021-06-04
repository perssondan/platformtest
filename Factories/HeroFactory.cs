using Microsoft.Graphics.Canvas;
using System;
using System.Numerics;
using uwpPlatformer.Components;
using uwpPlatformer.GameObjects;
using Windows.Foundation;

namespace uwpPlatformer.Factories
{
    public class HeroFactory
    {
        public GameObject CreateHero(CanvasBitmap canvasBitmap, Vector2 position, Vector2 size)
        {
            var gameObject = new GameObject();
            gameObject.AddOrUpdateComponent(new TransformComponent(gameObject)
            {
                Position = position
            });
            var sourceRects = new Rect[]
            {
                new Rect(0,32f,32,32),
                new Rect(32f,32f,32f,32f),
                new Rect(64f,32f,32f,32f),
                new Rect(96f,32f,32f,32f),
            };
            var animatedGraphicsComponent = new AnimatedGraphicsComponent(gameObject, canvasBitmap, sourceRects, TimeSpan.FromMilliseconds(150));
            gameObject.AddOrUpdateComponent(animatedGraphicsComponent);
            gameObject.AddOrUpdateComponent(new PhysicsComponent(gameObject));
            gameObject.AddOrUpdateComponent(new InputComponent(gameObject));
            gameObject.AddOrUpdateComponent(new PlayerComponent(gameObject));
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
