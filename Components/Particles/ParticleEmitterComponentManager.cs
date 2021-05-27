namespace uwpPlatformer.Components.Particles
{
    public class ParticleEmitterComponentManager : ComponentManager<ParticleEmitterComponent>
    {
        protected ParticleEmitterComponentManager()
        {
        }

        public static ParticleEmitterComponentManager Instance { get; } = new ParticleEmitterComponentManager();
    }
}
