using GamesLibrary.Models;
using GamesLibrary.Systems;
using System;
using System.Linq;
using uwpPlatformer.Components;
using uwpPlatformer.Components.Particles;
using uwpPlatformer.Extensions;
using uwpPlatformer.Factories;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Systems
{
    public class DustParticleEmitterSystem : ISystem
    {
        private readonly IEventSystem _eventSystem;
        private readonly DustEntityFactory _dustEntityFactory;
        private readonly IGameObjectManager _gameObjectManager;

        public DustParticleEmitterSystem(IEventSystem eventSystem,
                                         DustEntityFactory dustEntityFactory,
                                         IGameObjectManager gameObjectManager)
        {
            _dustEntityFactory = dustEntityFactory;
            _gameObjectManager = gameObjectManager;
            _eventSystem = eventSystem;
        }

        public string Name => nameof(DustParticleEmitterSystem);

        public void Update(TimingInfo timingInfo)
        {
            _gameObjectManager.GameObjects
                .Select(gameObject => gameObject.GetComponents<DustParticleEmitterComponent, TransformComponent>())
                .Where(result => result != default)
                .ToArray() // clone
                .ForEach(result =>
                {
                    CreateParticles(result.Item1, result.Item2, timingInfo.TotalTime);
                    if (result.Item1.ParticleEmitterType == ParticleEmitterType.Burst)
                    {
                        result.Item1.GameObject.RemoveComponent<DustParticleEmitterComponent>();
                    }
                });
        }

        private void CreateParticles(DustParticleEmitterComponent particleEmitterComponent,
                                     TransformComponent transformComponent,
                                     TimeSpan createdAt)
        {
            if (particleEmitterComponent.NumberOfParticles > 0 &&
                particleEmitterComponent.ParticleEmitterType != ParticleEmitterType.None)
            {
                var position = transformComponent.Position + particleEmitterComponent.ParticleOffset;
                _dustEntityFactory.CreateDustParticleEntitesAndUnwrap(position,
                                                                      createdAt,
                                                                      particleEmitterComponent.NumberOfParticles,
                                                                      particleEmitterComponent.TimeToLive,
                                                                      particleEmitterComponent.InitialVelocityFactor);
            }
        }
    }
}
