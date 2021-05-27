namespace uwpPlatformer.Components.Particles
{
    public class DustParticleEmitterComponentManager : ComponentManager<DustParticleEmitterComponent>
    {
        protected DustParticleEmitterComponentManager()
        {
        }

        public static DustParticleEmitterComponentManager Instance { get; } = new DustParticleEmitterComponentManager();
    }
}
