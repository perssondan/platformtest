using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using uwpPlatformer.Components;
using uwpPlatformer.GameObjects;
using Windows.UI;

namespace uwpPlatformer.Factories
{
    public class DustEntityFactory
    {
        private static readonly Random _random = new Random((int)DateTime.Now.Ticks);

        public void CreateDustParticleEntitesAndUnwrap(Vector2 position,
                                                       int numberOfParticles,
                                                       TimeSpan timeToLive,
                                                       TimeSpan createdAt,
                                                       float initialVelocityFactor = 75f)
        {
            CreateDustParticleEntites(position,
                                      numberOfParticles,
                                      timeToLive,
                                      createdAt,
                                      initialVelocityFactor)
                .ToArray();
        }

        public IEnumerable<GameObject> CreateDustParticleEntites(Vector2 position,
                                                                 int numberOfParticles,
                                                                 TimeSpan timeToLive,
                                                                 TimeSpan createdAt,
                                                                 float initialVelocityFactor = 75f)
        {
            for (var particleIndex = 0; particleIndex < numberOfParticles; particleIndex++)
            {
                var gameObject = new GameObject();
                gameObject.TransformComponent.Position = position;
                var velocity = CreateRandomVelocityVector();

                // set a initial speed with some randomness
                gameObject.TransformComponent.Velocity = (float)_random.NextDouble() * initialVelocityFactor * velocity;

                // TODO: Where's a good place to have the color shading stuff?
                gameObject.AddComponent(new ParticleComponent(gameObject, timeToLive, Colors.White, Colors.WhiteSmoke, ChangeColorBehavior.OverTime, createdAt));
                gameObject.AddComponent(new ShapeGraphicsComponent(gameObject, ShapeType.Rectangle, Colors.White, new Vector2(2f, 2f)));

                yield return gameObject;
            }
        }

        private static Vector2 CreateRandomVelocityVector()
        {
            const double phi = 2f * Math.PI;

            var randValue = _random.NextDouble();
            var x = Math.Cos(randValue * phi);
            var y = Math.Sin(randValue * phi);
            var velocity = new Vector2((float)x, (float)y);
            return velocity;
        }
    }
}
