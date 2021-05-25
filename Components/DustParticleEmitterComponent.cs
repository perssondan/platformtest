using System;
using System.Numerics;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Components
{
    public struct DustParticleEmitterComponent : IComponent
    {
        public DustParticleEmitterComponent(TimeSpan timeToLive,
                                            GameObject gameObject,
                                            Vector2 particleOffset,
                                            ushort numberOfParticles,
                                            float initialVelocityFactor)
        {
            TimeToLive = timeToLive;
            GameObject = gameObject;
            ParticleOffset = particleOffset;
            NumberOfParticles = numberOfParticles;
            InitialVelocityFactor = initialVelocityFactor;

            DustParticleEmitterComponentManager.Instance.AddComponent(this);
        }

        public TimeSpan TimeToLive;
        public Vector2 ParticleOffset;
        public float InitialVelocityFactor;
        public ushort NumberOfParticles;

        public GameObject GameObject { get; set; }

        public void Dispose()
        {
            DustParticleEmitterComponentManager.Instance.RemoveComponent(this);
        }
    }
}
