using GamesLibrary.Models;
using GamesLibrary.Systems;
using System;
using System.Collections.Generic;
using uwpPlatformer.Components;
using uwpPlatformer.EventArguments;
using uwpPlatformer.Extensions;
using uwpPlatformer.Factories;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Systems
{
    public class DustParticleEmitterSystem : SystemBase<DustParticleEmitterSystem>
    {
        private readonly IEventSystem _eventSystem;
        private readonly DustEntityFactory _dustEntityFactory;
        private readonly HashSet<GameObject> _activeEmitters = new HashSet<GameObject>();

        public DustParticleEmitterSystem(IEventSystem eventSystem)
        {
            _dustEntityFactory = new DustEntityFactory();
            _eventSystem = eventSystem;

            _eventSystem.Subscribe<ActivateDustParticles>(this, (sender, activateEmitter) =>
            {
                // Should we keep this here, or is it a setting on the emitter?
                _activeEmitters.Add(activateEmitter.GameObject);
            });
        }

        public override void Update(TimingInfo timingInfo)
        {
            // TODO: In case of bursts or continious we need to track when it's time
            // to create new particles
            _activeEmitters.ForEach(gameObject => CreateParticles(gameObject, timingInfo.TotalTime));

            _activeEmitters.Clear();
        }

        private void CreateParticles(GameObject gameObject, TimeSpan createdAt)
        {
            var particleEmitterComponent = gameObject.GetComponent<DustParticleEmitterComponent>();
            if (particleEmitterComponent.NumberOfParticles > 0)
            {
                var position = gameObject.TransformComponent.Position + particleEmitterComponent.ParticleOffset;
                _dustEntityFactory.CreateDustParticleEntitesAndUnwrap(position,
                                                                      particleEmitterComponent.NumberOfParticles,
                                                                      particleEmitterComponent.TimeToLive,
                                                                      createdAt,
                                                                      particleEmitterComponent.InitialVelocityFactor);
            }
        }
    }
}
