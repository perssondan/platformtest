using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Components
{
    public class EnemyComponent : ComponentBase, IComponent
    {
        public EnemyComponent(GameObject gameObject)
            : base(gameObject)
        {
        }
    }
}
