using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Components.Particles
{
    public struct DustParticleEmitterComponent : IComponent
    {
        public DustParticleEmitterComponent(TimeSpan timeToLive,
                                            GameObject gameObject,
                                            Vector2 particleOffset,
                                            ushort numberOfParticles,
                                            float initialVelocityFactor,
                                            ParticleEmitterType particleEmitterType)
        {
            TimeToLive = timeToLive;
            GameObject = gameObject;
            ParticleOffset = particleOffset;
            NumberOfParticles = numberOfParticles;
            InitialVelocityFactor = initialVelocityFactor;
            ParticleEmitterType = particleEmitterType;
        }

        public TimeSpan TimeToLive;
        public Vector2 ParticleOffset;
        public float InitialVelocityFactor;
        public ushort NumberOfParticles;
        public ParticleEmitterType ParticleEmitterType;

        public GameObject GameObject { get; set; }

        public void Dispose()
        {
        }

        public override bool Equals(object obj)
        {
            return obj is DustParticleEmitterComponent component &&
                   TimeToLive.Equals(component.TimeToLive) &&
                   ParticleOffset.Equals(component.ParticleOffset) &&
                   InitialVelocityFactor == component.InitialVelocityFactor &&
                   NumberOfParticles == component.NumberOfParticles &&
                   GameObject == component.GameObject;
        }

        public override int GetHashCode()
        {
            int hashCode = TimeToLive.GetHashCode();
            hashCode = HashCodeHelper.CombineHashCodes(hashCode, ParticleOffset.GetHashCode());
            hashCode = HashCodeHelper.CombineHashCodes(hashCode, InitialVelocityFactor.GetHashCode());
            hashCode = HashCodeHelper.CombineHashCodes(hashCode, NumberOfParticles.GetHashCode());
            hashCode = HashCodeHelper.CombineHashCodes(hashCode, GameObject?.GetHashCode() ?? 0);

            return hashCode;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(DustParticleEmitterComponent left, DustParticleEmitterComponent right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(DustParticleEmitterComponent left, DustParticleEmitterComponent right)
        {
            return !(left == right);
        }
    }
}
