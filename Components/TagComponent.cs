using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Components
{
    public class TagComponent : ComponentBase, IComponent
    {
        public TagComponent(GameObject gameObject, string tag) : base(gameObject)
        {
            Tag = tag;
        }

        public string Tag { get; }
    }
}