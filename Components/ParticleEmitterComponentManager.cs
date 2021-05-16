namespace uwpKarate.Components
{
    public class ParticleEmitterComponentManager : ComponentManager<ParticleEmitterComponent>
    {
        protected ParticleEmitterComponentManager()
        {
        }

        public static ParticleEmitterComponentManager Instance { get; } = new ParticleEmitterComponentManager();
    }
}
