using GamesLibrary.Models;
using System;
using System.Linq;
using uwpPlatformer.Components;
using uwpPlatformer.Components.Particles;
using uwpPlatformer.Extensions;
using uwpPlatformer.Factories;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Systems
{
    public class ParticleEmitterSystem : SystemBase<ParticleEmitterSystem>
    {
        private DustEntityFactory _dustEntityFactory;

        public ParticleEmitterSystem(DustEntityFactory dustEntityFactory)
        {
            _dustEntityFactory = dustEntityFactory;
        }

        public override void Update(TimingInfo timingInfo)
        {
            GameObjectManager.GameObjects
                .Select(gameObject => gameObject.GetComponents<ParticleEmitterComponent, TransformComponent>())
                .Where(result => result != default)
                .ToArray() // clone
                .ForEach(result =>
                {
                    CreateParticles(result.Item1, result.Item2, timingInfo.TotalTime);
                    if (result.Item1.ParticleEmitterType == ParticleEmitterType.Burst)
                    {
                        result.Item1.GameObject.RemoveComponent<ParticleEmitterComponent>();
                    }
                });
        }

        private void CreateParticles(ParticleEmitterComponent particleEmitterComponent,
                                     TransformComponent transformComponent,
                                     TimeSpan createdAt)
        {
            if (particleEmitterComponent.ParticleTemplateType == ParticleTemplateType.Dust &&
                particleEmitterComponent.ParticleEmitterType != ParticleEmitterType.None)
            {
                var position = transformComponent.Position + particleEmitterComponent.ParticleOffset;
                _dustEntityFactory.CreateDustParticleEntitesAndUnwrap(position,
                                                                      createdAt);
            }
        }
    }
}
