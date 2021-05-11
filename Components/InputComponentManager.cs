namespace uwpKarate.Components
{
    public class InputComponentManager : ComponentManager<InputComponent>
    {
        protected InputComponentManager()
        {
        }

        public static InputComponentManager Instance { get; } = new InputComponentManager();
    }
}