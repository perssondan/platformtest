using uwpKarate.GameObjects;

namespace uwpKarate.Components
{
    public class ParticleEmitterComponent : GameObjectComponent, IComponent
    {
        public ParticleEmitterComponent(GameObject gameObject) : base(gameObject)
        {
            ParticleEmitterComponentManager.Instance.AddComponent(this);
        }

        protected override void OnDispose()
        {
            ParticleEmitterComponentManager.Instance.RemoveComponent(this);
        }
    }
}
