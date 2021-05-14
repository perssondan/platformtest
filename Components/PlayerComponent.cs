using uwpKarate.GameObjects;

namespace uwpKarate.Components
{
    public class PlayerComponent : GameObjectComponent, IComponent
    {
        public PlayerComponent(GameObject gameObject)
            : base(gameObject)
        {
        }
    }
}