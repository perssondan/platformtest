using uwpKarate.GameObjects;

namespace uwpKarate.Components
{
    public class PlayerComponent : GameObjectComponent, IGameObjectComponent
    {
        public PlayerComponent(GameObject gameObject)
            : base(gameObject)
        {
        }
    }
}