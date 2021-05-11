namespace uwpKarate.Components
{
    public struct ParticleComponent
    {
        public ParticleComponent(int numberOfParticles = 0)
        {
            NumberOfParticles = numberOfParticles;
        }

        public int NumberOfParticles { get; set; }

        //protected override void OnDispose()
        //{
        //    ParticleComponentManager.Instance.RemoveComponent(this);
        //}
    }
}
