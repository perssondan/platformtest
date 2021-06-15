using GamesLibrary.Models;
using GamesLibrary.Utilities;
using System;
using System.Linq;
using System.Numerics;
using uwpPlatformer.Components;
using uwpPlatformer.Extensions;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Systems
{
    public class PerlinSystem : SystemBase<PerlinSystem>
    {
        private readonly PerlinNoise _perlinNoise;
        private readonly IGameObjectManager _gameObjectManager;

        public PerlinSystem(IGameObjectManager gameObjectManager)
        {
            _perlinNoise = new PerlinNoise();
            _gameObjectManager = gameObjectManager;
        }

        public override void Update(TimingInfo timingInfo)
        {
            _gameObjectManager.GameObjects
                .Select(gameObject => (gameObject, components: gameObject.GetComponents<PerlinMovementComponent, TransformComponent, PhysicsComponent>()))
                .Where(result => result != default && result.Item2 != default)
                .ToArray() // clone
                .ForEach(result =>
                {
                    //UpdateEntityPosition(result.gameObject, result.Item2.Item1, result.Item2.Item2);
                    //UpdateEntityVelocity(result.gameObject, result.components.Item1, result.components.Item2);
                    UpdateEntityForce(result.gameObject, result.components.Item1, result.components.Item2, result.components.Item3);
                });
        }

        private void UpdateEntityPosition(GameObject gameObject, PerlinMovementComponent perlinComponent, TransformComponent transform)
        {
            var perlinX = _perlinNoise.Noise(new Vector3(perlinComponent.OffsetX, perlinComponent.OffsetX, 0f));
            perlinComponent.OffsetX += 0.01f;
            var perlinY = _perlinNoise.Noise(new Vector3(perlinComponent.OffsetY, perlinComponent.OffsetY, 0f));
            perlinComponent.OffsetY += 0.01f;
            transform.Position = new Vector2(
                (float)GameMath.Map((float)perlinX, -1f, 1f, (float)perlinComponent.Bounds.Left, (float)perlinComponent.Bounds.Right),
                (float)GameMath.Map((float)perlinY, -1f, 1f, (float)perlinComponent.Bounds.Top, (float)perlinComponent.Bounds.Bottom));


            // replacing, since I started to use struct as type for my new components...
            gameObject.AddOrUpdateComponent(perlinComponent);
        }

        private void UpdateEntityVelocity(GameObject gameObject, PerlinMovementComponent perlinComponent, TransformComponent transform)
        {
            var angle = _perlinNoise.Noise(new Vector3(perlinComponent.OffsetX, perlinComponent.OffsetY, 0f)) * (float)Math.PI * 2;
            var speedVector = angle.VectorFromAngle();

            transform.Velocity = (speedVector * 60);

            perlinComponent.OffsetX += 0.01f;
            perlinComponent.OffsetY += 0.01f;

            // replacing, since I started to use struct as type for my new components...
            gameObject.AddOrUpdateComponent(perlinComponent);
        }

        private void UpdateEntityForce(GameObject gameObject, PerlinMovementComponent perlinComponent, TransformComponent transform, PhysicsComponent physicsComponent)
        {
            var deriv = Vector3.Zero;
            var angle = _perlinNoise.Noise(new Vector3(perlinComponent.OffsetX, perlinComponent.OffsetY, 0f)) * (float)Math.PI * 2;
            var forceVector = angle.VectorFromAngle();

            physicsComponent.ImpulseForce += (forceVector * 80f);

            perlinComponent.OffsetX += 0.01f;
            perlinComponent.OffsetY += 0.01f;

            // replacing, since I started to use struct as type for my new components...
            gameObject.AddOrUpdateComponent(perlinComponent);
        }
    }
}
