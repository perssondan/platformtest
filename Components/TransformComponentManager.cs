namespace uwpKarate.Components
{
    public class TransformComponentManager : ComponentManager<TransformComponent>
    {
        protected TransformComponentManager()
        {
        }

        public static TransformComponentManager Instance { get; } = new TransformComponentManager();
    }
}