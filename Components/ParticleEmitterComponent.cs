using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Components
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
