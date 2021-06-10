using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Components
{
    public abstract class ComponentBase : IComponent
    {
        public ComponentBase(GameObject gameObject)
        {
            GameObject = gameObject;
        }

        public GameObject GameObject { get; set; }
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            GameObject = null;
            IsDisposed = true;
        }
    }
}
