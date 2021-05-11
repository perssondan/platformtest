using System;
using uwpKarate.GameObjects;

namespace uwpKarate.Components
{
    public class ParticleComponent : GameObjectComponent
    {
        public ParticleComponent(GameObject gameObject) : base(gameObject)
        {
            ParticleComponentManager.Instance.AddComponent(this);
        }

        public int NumberOfParticles { get; set; }
        public TimeSpan TimeToLive { get; set; }
        public TimeSpan EllapsedTime { get; set; } = TimeSpan.Zero;

        protected override void OnDispose()
        {
            ParticleComponentManager.Instance.RemoveComponent(this);
        }
    }
}
