namespace uwpKarate.Components
{
    public class ColliderComponentManager : ComponentManager<ColliderComponent>
    {
        protected ColliderComponentManager()
        {
        }

        public static ColliderComponentManager Instance { get; } = new ColliderComponentManager();
    }
}