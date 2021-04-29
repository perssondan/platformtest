using uwpKarate.GameObjects;

namespace uwpKarate.Components
{
    public class GameObjectComponent : IGameObjectComponent
    {
        public GameObjectComponent(GameObject gameObject)
        {
            GameObject = gameObject;
        }

        protected GameObject GameObject { get; }
    }
}
