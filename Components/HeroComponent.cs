using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Components
{
    public class HeroComponent : ComponentBase, IComponent
    {
        public HeroComponent(GameObject gameObject)
            : base(gameObject)
        {
        }
    }
}
