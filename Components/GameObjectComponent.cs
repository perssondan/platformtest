using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Components
{
    public abstract class GameObjectComponent : IComponent
    {
        public GameObjectComponent(GameObject gameObject)
        {
            GameObject = gameObject;
        }

        public GameObject GameObject { get; set; }
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            OnDispose();
            GameObject = null;
            IsDisposed = true;
        }

        protected virtual void OnDispose()
        {

        }
    }
}
