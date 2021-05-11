using uwpKarate.GameObjects;
using uwpKarate.Systems;

namespace uwpKarate.Components
{
    public class InputComponent : GameObjectComponent, IGameObjectComponent
    {
        public InputComponent(GameObject gameObject)
            : base(gameObject)
        {
            InputComponentManager.Instance.AddComponent(this);
        }

        protected override void OnDispose()
        {
            InputComponentManager.Instance.RemoveComponent(this);
        }

        public UserInput UserInputs { get; set; }
    }
}