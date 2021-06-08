using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using uwpPlatformer.Components;
using uwpPlatformer.GameObjects;
using uwpPlatformer.Models;
using Windows.Foundation;

namespace uwpPlatformer.Factories
{
    public class TileFactory
    {
        public GameObject CreateTile(CanvasBitmap canvasBitmap, Vector2 position, Vector2 size, TileType tileType)
        {
            var gameObject = new GameObject();
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
}
