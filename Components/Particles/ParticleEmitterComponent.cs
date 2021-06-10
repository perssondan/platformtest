using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Components.Particles
{
    public struct ParticleEmitterComponent : IComponent
    {
        public ParticleEmitterComponent(
            GameObject gameObject,
            ParticleTemplateType particleTemplateType,
            Vector2 particleOffset)
        {
            GameObject = gameObject;
            ParticleTemplateType = particleTemplateType;
            switch (particleTemplateType)
            {
                case ParticleTemplateType.Dust:
                case ParticleTemplateType.Explosion:
                    ParticleEmitterType = ParticleEmitterType.Burst;
                    break;
                case ParticleTemplateType.None:
                default:
                    ParticleEmitterType = ParticleEmitterType.None;
                    break;
            }
            ParticleOffset = particleOffset;
        }

        public Vector2 ParticleOffset;
        public ParticleTemplateType ParticleTemplateType;
        public ParticleEmitterType ParticleEmitterType;

        public GameObject GameObject { get; set; }

        public void Dispose()
        {
        }

        public override bool Equals(object obj)
        {
            return obj is ParticleEmitterComponent component &&
                   ParticleOffset.Equals(component.ParticleOffset) &&
                   GameObject == component.GameObject;
        }

        public override int GetHashCode()
        {
            int hashCode = ParticleTemplateType.GetHashCode();
            hashCode = HashCodeHelper.CombineHashCodes(hashCode, ParticleOffset.GetHashCode());
            hashCode = HashCodeHelper.CombineHashCodes(hashCode, ParticleEmitterType.GetHashCode());
            hashCode = HashCodeHelper.CombineHashCodes(hashCode, GameObject?.GetHashCode() ?? 0);

            return hashCode;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(ParticleEmitterComponent left, ParticleEmitterComponent right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(ParticleEmitterComponent left, ParticleEmitterComponent right)
        {
            return !(left == right);
        }
    }
}
