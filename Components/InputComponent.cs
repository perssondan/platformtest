using uwpPlatformer.GameObjects;
using uwpPlatformer.Systems;

namespace uwpPlatformer.Components
{
    public class InputComponent : ComponentBase, IComponent
    {
        public InputComponent(GameObject gameObject)
            : base(gameObject)
        {
        }

        public UserInput UserInputs { get; set; }
    }
}