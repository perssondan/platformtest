using Microsoft.Graphics.Canvas;
using System;
using System.Numerics;
using uwpPlatformer.Components;
using uwpPlatformer.GameObjects;
using Windows.Foundation;
using Windows.UI;

namespace uwpPlatformer.Factories
{
    public class EnemyFactory
    {
        public GameObject CreateFlyingEnemy(CanvasBitmap canvasBitmap, Vector2 position, Vector2 size, Rect bounds)
        {
            var gameObject = new GameObject();
            gameObject.TransformComponent.Position = position;

            //gameObject.AddComponent(new ShapeGraphicsComponent(gameObject, ShapeType.Rectangle, Colors.Pink, size));
            gameObject.AddOrUpdateComponent(new AnimatedGraphicsComponent(gameObject, canvasBitmap, new[] { new Rect(new Point(0, 0), size.ToSize()) }, TimeSpan.Zero));
            gameObject.AddOrUpdateComponent(new PerlinMovementComponent(gameObject, bounds, 0f, 1000f));

            return gameObject;
        }
    }
}
