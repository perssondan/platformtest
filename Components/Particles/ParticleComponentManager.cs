namespace uwpPlatformer.Components.Particles
{
    public class ParticleComponentManager : ComponentManager<ParticleComponent>
    {
        protected ParticleComponentManager()
        {
        }

        public static ParticleComponentManager Instance { get; } = new ParticleComponentManager();
    }
}
