﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using uwpPlatformer.Components;
using uwpPlatformer.Components.Particles;
using uwpPlatformer.GameObjects;
using Windows.UI;

namespace uwpPlatformer.Factories
{
    public class DustEntityFactory
    {
        private static readonly Random _random = new Random((int)DateTime.Now.Ticks);

        public void CreateDustParticleEntitesAndUnwrap(Vector2 position,
                                                       TimeSpan createdAt,
                                                       int numberOfParticles = 10,
                                                       TimeSpan? timeToLive = null,
                                                       float initialVelocityFactor = 50f)
        {
            CreateDustParticleEntites(position,
                                      numberOfParticles,
                                      timeToLive ?? TimeSpan.FromSeconds(.3),
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

                // TODO: Where's a good place to have the color shading stuff, separate component?
                gameObject.AddOrUpdateComponent(new ParticleComponent(gameObject, timeToLive, Colors.White, Colors.SandyBrown, TransitionBehavior.OverTime, createdAt) { FadeBehavior = FadeBehavior.FadeOut });
                gameObject.AddOrUpdateComponent(new ShapeGraphicsComponent(gameObject, ShapeType.Rectangle, Colors.White, new Vector2(2f, 2f)));

                yield return gameObject;
            }
        }

        private static Vector2 CreateRandomVelocityVector()
        {
            const double phi = 2f * Math.PI;

            var randomValue = _random.NextDouble();
            var x = Math.Cos(randomValue * phi);
            var y = Math.Sin(randomValue * phi);
            var velocity = new Vector2((float)x, (float)y);
            return velocity;
        }
    }
}
